// Invoicing/Domain/Model/Commands/CorregirDatoFacturaCommand.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.Commands;

/// <summary>
///     Corrige uno o más campos de una factura mal leídos por la IA. Solo se actualizan
///     los campos que llegan distintos de null.
/// </summary>
public record CorregirDatoFacturaCommand(
    Guid FacturaId,
    Guid UsuarioId,
    string? ProveedorRuc,
    string? ProveedorRazonSocial,
    Guid? CategoriaId,
    string? Serie,
    string? Numero,
    DateTime? FechaEmision,
    decimal? MontoTotal,
    string? Moneda);