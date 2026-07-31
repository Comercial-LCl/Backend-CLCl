// Invoicing/Interfaces/Rest/Resources/CorregirDatoFacturaResource.cs
namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

/// <summary>
///     Todos los campos son opcionales — solo se corrige lo que llega distinto de null.
/// </summary>
public record CorregirDatoFacturaResource(
    string? ProveedorRuc,
    string? ProveedorRazonSocial,
    Guid? CategoriaId,
    string? Serie,
    string? Numero,
    DateTime? FechaEmision,
    decimal? MontoTotal,
    string? Moneda);