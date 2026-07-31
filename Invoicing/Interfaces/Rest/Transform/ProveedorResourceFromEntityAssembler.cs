// Invoicing/Interfaces/Rest/Transform/ProveedorResourceFromEntityAssembler.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;

public static class ProveedorResourceFromEntityAssembler
{
    public static ProveedorResource ToResourceFromEntity(Proveedor entity)
    {
        return new ProveedorResource(entity.Id, entity.Ruc.Valor, entity.RazonSocial);
    }
}