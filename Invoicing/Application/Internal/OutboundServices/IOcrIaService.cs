// Invoicing/Application/Internal/OutboundServices/IOcrIaService.cs
namespace FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;

public record ItemExtraido(string Descripcion, decimal Cantidad, decimal PrecioUnitario);

/// <summary>
///     Resultado de procesar una factura física (foto) — la cabecera ya vino del QR,
///     Gemini solo extrae el detalle de ítems y clasifica.
/// </summary>
public record ResultadoOcrIaFisica(
    IReadOnlyCollection<ItemExtraido> Items,
    string CategoriaSugerida,
    string ResumenIa);

/// <summary>
///     Resultado de procesar una factura electrónica (PDF) — Gemini extrae también la
///     cabecera a partir del texto ya extraído del PDF.
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
    string ResumenIa);

/// <summary>
///     Outbound service para Google Gemini (OCR multimodal + clasificación + resumen)
/// </summary>
public interface IOcrIaService
{
    Task<ResultadoOcrIaFisica> ProcesarFacturaFisicaAsync(
        byte[] imagenBytes, string contentType, CancellationToken cancellationToken);

    Task<ResultadoIaElectronica> ProcesarFacturaElectronicaAsync(
        string textoExtraido, CancellationToken cancellationToken);
}