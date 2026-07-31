// Invoicing/Domain/Model/Commands/RegistrarFacturaElectronicaCommand.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.Commands;

/// <param name="UsuarioId">Dueño de negocio que registra la factura</param>
/// <param name="ArchivoPdfBytes">El PDF de la factura electrónica</param>
/// <param name="NombreArchivo">Nombre original del archivo</param>
public record RegistrarFacturaElectronicaCommand(Guid UsuarioId, byte[] ArchivoPdfBytes, string NombreArchivo);