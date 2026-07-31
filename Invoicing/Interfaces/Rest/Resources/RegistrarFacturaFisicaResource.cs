// Invoicing/Interfaces/Rest/Resources/RegistrarFacturaFisicaResource.cs
namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

/// <summary>
///     Datos leídos del QR de una factura física. La foto se envía aparte como archivo (multipart).
/// </summary>
public record RegistrarFacturaFisicaResource(
    string ProveedorRuc,
    string Serie,
    string Numero,
    DateTime FechaEmision,
    decimal MontoTotal,
    string Moneda);