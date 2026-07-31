// Invoicing/Interfaces/Rest/Transform/CorregirDatoFacturaCommandFromResourceAssembler.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Commands;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;

public static class CorregirDatoFacturaCommandFromResourceAssembler
{
    public static CorregirDatoFacturaCommand ToCommandFromResource(
        Guid facturaId, Guid usuarioId, CorregirDatoFacturaResource resource)
    {
        return new CorregirDatoFacturaCommand(
            facturaId,
            usuarioId,
            resource.ProveedorRuc?.Trim(),
            resource.ProveedorRazonSocial?.Trim(),
            resource.CategoriaId,
            resource.Serie?.Trim(),
            resource.Numero?.Trim(),
            resource.FechaEmision,
            resource.MontoTotal,
            resource.Moneda?.Trim());
    }
}