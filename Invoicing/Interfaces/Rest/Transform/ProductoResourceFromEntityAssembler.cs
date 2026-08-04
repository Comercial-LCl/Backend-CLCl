// Invoicing/Interfaces/Rest/Transform/ProductoResourceFromEntityAssembler.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;

public static class ProductoResourceFromEntityAssembler
{
    public static ProductoResource ToResourceFromEntity(Producto entity)
    {
        return new ProductoResource(entity.Id, entity.ProveedorId, entity.Nombre);
    }
}