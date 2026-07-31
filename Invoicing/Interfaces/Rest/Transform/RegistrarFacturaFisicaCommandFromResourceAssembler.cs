// Invoicing/Interfaces/Rest/Transform/RegistrarFacturaFisicaCommandFromResourceAssembler.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Commands;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;

public static class RegistrarFacturaFisicaCommandFromResourceAssembler
{
    public static RegistrarFacturaFisicaCommand ToCommandFromResource(
        RegistrarFacturaFisicaResource resource, Guid usuarioId, byte[]? imagenBytes, string? imagenContentType)
    {
        return new RegistrarFacturaFisicaCommand(
            usuarioId,
            resource.ProveedorRuc.Trim(),
            resource.Serie.Trim(),
            resource.Numero.Trim(),
            resource.FechaEmision,
            resource.MontoTotal,
            resource.Moneda.Trim(),
            imagenBytes,
            imagenContentType);
    }
}