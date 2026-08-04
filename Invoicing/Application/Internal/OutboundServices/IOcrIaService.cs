// Invoicing/Application/Internal/OutboundServices/IOcrIaService.cs — reemplaza el archivo completo
using FacturasIA.Platform.Invoicing.Domain.Model;

namespace FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;

/// <param name="Descripcion">Texto tal cual lo lee la IA, para mostrarlo al usuario en el detalle.</param>
/// <param name="NombreNormalizado">Versión corta y estandarizada (ej. "papel bond a4"), usada para
///     resolver/crear el Producto — evita que "Papel Bond A4" y "papel bond a4 75gr" sean productos distintos.</param>
public record ItemExtraido(string Descripcion, string NombreNormalizado, decimal Cantidad, decimal PrecioUnitario);

/// <summary>
///     Resultado de procesar una factura física (foto) — la cabecera ya vino del QR, así que la
///     única confianza que aplica aquí es la de la categoría sugerida (lo único que la IA decide).
/// </summary>
public record ResultadoOcrIaFisica(
    IReadOnlyCollection<ItemExtraido> Items,
    string CategoriaSugerida,
    string ResumenIa,
    NivelConfianza ConfianzaCategoria,
    bool ItemsRequierenRevision);

/// <summary>
///     Resultado de procesar una factura electrónica (PDF) — Gemini extrae también la cabecera,
///     así que autoevalúa su confianza en cada campo de cabecera además de la categoría.
/// </summary>
public record ResultadoIaElectronica(
    string ProveedorRuc,
    string ProveedorRazonSocial,
    string Serie,
    string Numero,
    DateTime FechaEmision,
    decimal MontoTotal,
    string Moneda,
    IReadOnlyCollection<ItemExtraido> Items,
    string CategoriaSugerida,
    string ResumenIa,
    IReadOnlyDictionary<string, NivelConfianza> ConfianzaCampos,
    bool ItemsRequierenRevision);

public interface IOcrIaService
{
    Task<ResultadoOcrIaFisica> ProcesarFacturaFisicaAsync(
        byte[] imagenBytes, string contentType, CancellationToken cancellationToken);

    Task<ResultadoIaElectronica> ProcesarFacturaElectronicaAsync(
        string textoExtraido, CancellationToken cancellationToken);
}