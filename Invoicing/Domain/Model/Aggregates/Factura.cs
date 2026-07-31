// Invoicing/Domain/Model/Aggregates/Factura.cs — reemplaza la clase completa
using FacturasIA.Platform.Invoicing.Domain.Model.Entities;
using FacturasIA.Platform.Invoicing.Domain.Model.ValueObjects;

namespace FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;

public class Factura
{
    private readonly List<ItemFactura> _items = [];

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid UsuarioId { get; private set; }
    public Guid ProveedorId { get; private set; }
    public Guid? CategoriaId { get; private set; }
    public TipoFactura Tipo { get; private set; }
    public NumeroFactura NumeroFactura { get; private set; } = null!;
    public DateTime FechaEmision { get; private set; }
    public Monto Monto { get; private set; } = null!;
    public string? ResumenIa { get; private set; }
    public string? ArchivoUrl { get; private set; }
    public EstadoProcesamiento EstadoProcesamiento { get; private set; } = EstadoProcesamiento.Pendiente;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public IReadOnlyCollection<ItemFactura> Items => _items.AsReadOnly();

    private Factura()
    {
    }

    public Factura(
        Guid usuarioId,
        Guid proveedorId,
        Guid? categoriaId,
        TipoFactura tipo,
        NumeroFactura numeroFactura,
        DateTime fechaEmision,
        Monto monto,
        string? archivoUrl)
    {
        UsuarioId = usuarioId;
        ProveedorId = proveedorId;
        CategoriaId = categoriaId;
        Tipo = tipo;
        NumeroFactura = numeroFactura;
        FechaEmision = fechaEmision;
        Monto = monto;
        ArchivoUrl = archivoUrl;
    }

    public void AgregarItem(ItemFactura item)
    {
        item.AsignarFactura(Id);
        _items.Add(item);
    }

    public void MarcarProcesada(string resumenIa)
    {
        ResumenIa = resumenIa;
        EstadoProcesamiento = EstadoProcesamiento.Procesado;
    }

    /// <summary>
    ///     Se usa cuando la factura física se registra sin foto: solo hay datos del QR,
    ///     sin resumen de IA, sin ítems y sin categoría (nadie la clasificó).
    /// </summary>
    public void MarcarProcesadaSinDetalle()
    {
        EstadoProcesamiento = EstadoProcesamiento.ProcesadoSinDetalle;
    }

    public void MarcarError()
    {
        EstadoProcesamiento = EstadoProcesamiento.Error;
    }

    public void Corregir(
        Guid? proveedorId,
        Guid? categoriaId,
        string? serie,
        string? numero,
        DateTime? fechaEmision,
        decimal? montoTotal,
        string? moneda)
    {
        if (proveedorId is not null) ProveedorId = proveedorId.Value;
        if (categoriaId is not null) CategoriaId = categoriaId.Value;

        if (serie is not null || numero is not null)
            NumeroFactura = new NumeroFactura(serie ?? NumeroFactura.Serie, numero ?? NumeroFactura.Numero);

        if (fechaEmision is not null) FechaEmision = fechaEmision.Value;

        if (montoTotal is not null || moneda is not null)
            Monto = new Monto(montoTotal ?? Monto.Valor, moneda ?? Monto.Moneda);
    }
}