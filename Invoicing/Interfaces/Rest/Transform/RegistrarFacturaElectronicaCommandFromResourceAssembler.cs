// Invoicing/Interfaces/Rest/Transform/RegistrarFacturaElectronicaCommandFromResourceAssembler.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Commands;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;

public static class RegistrarFacturaElectronicaCommandFromResourceAssembler
{
    public static RegistrarFacturaElectronicaCommand ToCommandFromResource(
        Guid usuarioId, byte[] archivoPdfBytes, string nombreArchivo)
    {
        return new RegistrarFacturaElectronicaCommand(usuarioId, archivoPdfBytes, nombreArchivo);
    }
}