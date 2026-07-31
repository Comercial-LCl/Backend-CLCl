// Invoicing/Interfaces/Rest/Transform/CategoriaResourceFromEntityAssembler.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;

public static class CategoriaResourceFromEntityAssembler
{
    public static CategoriaResource ToResourceFromEntity(Categoria entity)
    {
        return new CategoriaResource(entity.Id, entity.Nombre);
    }
}