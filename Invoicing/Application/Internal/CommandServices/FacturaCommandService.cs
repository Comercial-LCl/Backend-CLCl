// Invoicing/Application/Internal/CommandServices/FacturaCommandService.cs
using Microsoft.EntityFrameworkCore;
using FacturasIA.Platform.Invoicing.Application.CommandServices;
using FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;
using FacturasIA.Platform.Invoicing.Domain.Model;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Commands;
using FacturasIA.Platform.Invoicing.Domain.Model.Entities;
using FacturasIA.Platform.Invoicing.Domain.Model.ValueObjects;
using FacturasIA.Platform.Invoicing.Domain.Repositories;
using FacturasIA.Platform.Shared.Application.Model;
using FacturasIA.Platform.Shared.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FacturasIA.Platform.Invoicing.Application.Internal.CommandServices;

public class FacturaCommandService(
    IFacturaRepository facturaRepository,
    IProveedorRepository proveedorRepository,
    ICategoriaRepository categoriaRepository,
    IProductoRepository productoRepository,
    IOcrIaService ocrIaService,
    IAlmacenamientoService almacenamientoService,
    IPdfTextExtractorService pdfTextExtractorService,
    IConsultaRucService consultaRucService,
    IUnitOfWork unitOfWork,
    ILogger<FacturaCommandService> logger)
    : IFacturaCommandService
{
   async Task<Result<Factura>> IFacturaCommandService.Handle(
    RegistrarFacturaFisicaCommand command, CancellationToken cancellationToken)
{
    try
    {
        var ruc = new Ruc(command.ProveedorRuc);
        var proveedor = await ObtenerOCrearProveedorAsync(ruc, null, cancellationToken);

        Guid? categoriaId = null;
        string? archivoUrl = null;
        var items = new List<ItemFactura>();
        string? resumenIa = null;
        Dictionary<string, NivelConfianza>? confianzaCampos = null;
        var itemsRequierenRevision = false;

        if (command.ImagenBytes is not null && command.ImagenContentType is not null)
        {
            var resultadoIa = await ocrIaService.ProcesarFacturaFisicaAsync(
                command.ImagenBytes, command.ImagenContentType, cancellationToken);

            var categoria = await ObtenerOCrearCategoriaAsync(resultadoIa.CategoriaSugerida, cancellationToken);
            categoriaId = categoria.Id;
            resumenIa = resultadoIa.ResumenIa;
            confianzaCampos = new Dictionary<string, NivelConfianza> { ["categoria"] = resultadoIa.ConfianzaCategoria };
            itemsRequierenRevision = resultadoIa.ItemsRequierenRevision;

            foreach (var itemExtraido in resultadoIa.Items)
            {
                var producto = await ObtenerOCrearProductoAsync(proveedor.Id, itemExtraido.NombreNormalizado, cancellationToken);
                items.Add(new ItemFactura(producto.Id, itemExtraido.Descripcion, itemExtraido.Cantidad, itemExtraido.PrecioUnitario));
            }

            archivoUrl = await almacenamientoService.SubirImagenAsync(
                command.ImagenBytes, command.ImagenContentType, cancellationToken);
        }

        var factura = new Factura(
            command.UsuarioId,
            proveedor.Id,
            categoriaId,
            TipoFactura.Fisica,
            new NumeroFactura(command.Serie, command.Numero),
            command.FechaEmision,
            new Monto(command.MontoTotal, command.Moneda),
            archivoUrl);

        foreach (var item in items)
            factura.AgregarItem(item);

        if (resumenIa is not null)
            factura.MarcarProcesada(resumenIa, confianzaCampos!, itemsRequierenRevision);
        else
            factura.MarcarProcesadaSinDetalle();

        Factura? facturaCreada = null;
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await facturaRepository.AddAsync(factura, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            facturaCreada = factura;
        }, cancellationToken);

        return Result<Factura>.Success(facturaCreada!);
    }
    catch (OperationCanceledException)
    {
        return Result<Factura>.Failure(InvoicingError.OperationCancelled, "La operación fue cancelada.");
    }
    catch (DbUpdateException)
    {
        return Result<Factura>.Failure(InvoicingError.DatabaseError, "Ocurrió un error al guardar la factura.");
    }
    catch (ArgumentException ex)
    {
        return Result<Factura>.Failure(InvoicingError.InternalServerError, ex.Message);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error procesando factura con IA");
        return Result<Factura>.Failure(
            InvoicingError.ProcesamientoIaFallido, "No se pudo procesar la factura con IA.");
    }
}

    async Task<Result<Factura>> IFacturaCommandService.Handle(
        RegistrarFacturaElectronicaCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var textoExtraido = pdfTextExtractorService.ExtraerTexto(command.ArchivoPdfBytes);
            var resultadoIa = await ocrIaService.ProcesarFacturaElectronicaAsync(textoExtraido, cancellationToken);

            var ruc = new Ruc(resultadoIa.ProveedorRuc);
            var proveedor = await ObtenerOCrearProveedorAsync(ruc, resultadoIa.ProveedorRazonSocial, cancellationToken);
            var categoria = await ObtenerOCrearCategoriaAsync(resultadoIa.CategoriaSugerida, cancellationToken);

            var archivoUrl = await almacenamientoService.SubirPdfAsync(
                command.ArchivoPdfBytes, command.NombreArchivo, cancellationToken);

            var factura = new Factura(
                command.UsuarioId,
                proveedor.Id,
                categoria.Id,
                TipoFactura.Electronica,
                new NumeroFactura(resultadoIa.Serie, resultadoIa.Numero),
                resultadoIa.FechaEmision,
                new Monto(resultadoIa.MontoTotal, resultadoIa.Moneda),
                archivoUrl);

            foreach (var itemExtraido in resultadoIa.Items)
            {
                var producto = await ObtenerOCrearProductoAsync(proveedor.Id, itemExtraido.NombreNormalizado, cancellationToken);
                factura.AgregarItem(new ItemFactura(producto.Id, itemExtraido.Descripcion, itemExtraido.Cantidad, itemExtraido.PrecioUnitario));
            }

            factura.MarcarProcesada(resultadoIa.ResumenIa, resultadoIa.ConfianzaCampos, resultadoIa.ItemsRequierenRevision);
            Factura? facturaCreada = null;
            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await facturaRepository.AddAsync(factura, cancellationToken);
                await unitOfWork.CompleteAsync(cancellationToken);
                facturaCreada = factura;
            }, cancellationToken);

            return Result<Factura>.Success(facturaCreada!);
        }
        catch (OperationCanceledException)
        {
            return Result<Factura>.Failure(InvoicingError.OperationCancelled, "La operación fue cancelada.");
        }
        catch (DbUpdateException)
        {
            return Result<Factura>.Failure(InvoicingError.DatabaseError, "Ocurrió un error al guardar la factura.");
        }
        catch (ArgumentException ex)
        {
            return Result<Factura>.Failure(InvoicingError.InternalServerError, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error procesando factura con IA");
            return Result<Factura>.Failure(
                InvoicingError.ProcesamientoIaFallido, "No se pudo procesar la factura con IA.");
        }
    }

    async Task<Result<Factura>> IFacturaCommandService.Handle(
        CorregirDatoFacturaCommand command, CancellationToken cancellationToken)
    {
        var factura = await facturaRepository.FindByIdAsync(command.FacturaId, cancellationToken);
        if (factura is null)
            return Result<Factura>.Failure(InvoicingError.FacturaNotFound, "No se encontró la factura.");

        if (factura.UsuarioId != command.UsuarioId)
            return Result<Factura>.Failure(InvoicingError.NoAutorizado, "Esta factura no te pertenece.");

        try
        {
            Guid? proveedorId = null;
            if (command.ProveedorRuc is not null)
            {
                var ruc = new Ruc(command.ProveedorRuc);
                var proveedor = await ObtenerOCrearProveedorAsync(
                    ruc, command.ProveedorRazonSocial, cancellationToken);
                proveedorId = proveedor.Id;
            }

            factura.Corregir(
                proveedorId,
                command.CategoriaId,
                command.Serie,
                command.Numero,
                command.FechaEmision,
                command.MontoTotal,
                command.Moneda);

            facturaRepository.Update(factura);
            await unitOfWork.CompleteAsync(cancellationToken);

            return Result<Factura>.Success(factura);
        }
        catch (DbUpdateException)
        {
            return Result<Factura>.Failure(InvoicingError.DatabaseError, "Ocurrió un error al corregir la factura.");
        }
        catch (ArgumentException ex)
        {
            return Result<Factura>.Failure(InvoicingError.InternalServerError, ex.Message);
        }
    }

    private async Task<Proveedor> ObtenerOCrearProveedorAsync(
        Ruc ruc, string? razonSocial, CancellationToken cancellationToken)
    {
        var existente = await proveedorRepository.FindByRucAsync(ruc.Valor, cancellationToken);
        if (existente is not null)
            return razonSocial is not null ? existente.ActualizarRazonSocial(razonSocial) : existente;

        if (razonSocial is null)
        {
            var datosSunat = await consultaRucService.ConsultarAsync(ruc.Valor, cancellationToken);
            razonSocial = datosSunat?.RazonSocial;
        }

        var nuevo = new Proveedor(ruc, razonSocial ?? $"Proveedor {ruc.Valor}");
        await proveedorRepository.AddAsync(nuevo, cancellationToken);
        return nuevo;
    }
    
    private async Task<Producto> ObtenerOCrearProductoAsync(
        Guid proveedorId, string nombreNormalizado, CancellationToken cancellationToken)
    {
        var existente = await productoRepository.FindByProveedorAndNombreAsync(
            proveedorId, nombreNormalizado, cancellationToken);
        if (existente is not null) return existente;

        var nuevo = new Producto(proveedorId, nombreNormalizado);
        await productoRepository.AddAsync(nuevo, cancellationToken);
        return nuevo;
    }

    private async Task<Categoria> ObtenerOCrearCategoriaAsync(string nombre, CancellationToken cancellationToken)
    {
        var existente = await categoriaRepository.FindByNombreAsync(nombre, cancellationToken);
        if (existente is not null) return existente;

        var nueva = new Categoria(nombre);
        await categoriaRepository.AddAsync(nueva, cancellationToken);
        return nueva;
    }
}