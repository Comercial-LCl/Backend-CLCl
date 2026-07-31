// Invoicing/Interfaces/Rest/Transform/ResumenGastosResourceFromEntityAssembler.cs
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;

public static class ResumenGastosResourceFromEntityAssembler
{
    public static ResumenGastosPorCategoriaResource ToResourceFromEntity(ResumenPorCategoria entity)
    {
        return new ResumenGastosPorCategoriaResource(entity.CategoriaId, entity.CategoriaNombre, entity.Total);
    }
}