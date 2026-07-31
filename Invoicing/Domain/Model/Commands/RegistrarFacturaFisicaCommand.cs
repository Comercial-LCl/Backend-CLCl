// Invoicing/Domain/Model/Commands/RegistrarFacturaFisicaCommand.cs — ImagenBytes/ImagenContentType ahora opcionales
namespace FacturasIA.Platform.Invoicing.Domain.Model.Commands;

public record RegistrarFacturaFisicaCommand(
    Guid UsuarioId,
    string ProveedorRuc,
    string Serie,
    string Numero,
    DateTime FechaEmision,
    decimal MontoTotal,
    string Moneda,
    byte[]? ImagenBytes,
    string? ImagenContentType);