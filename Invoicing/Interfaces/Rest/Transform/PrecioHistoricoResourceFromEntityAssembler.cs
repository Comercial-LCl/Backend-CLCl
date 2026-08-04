// Invoicing/Interfaces/Rest/Transform/PrecioHistoricoResourceFromEntityAssembler.cs
using FacturasIA.Platform.Invoicing.Application.QueryServices;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;

public static class PrecioHistoricoResourceFromEntityAssembler
{
    public static PrecioHistoricoResource ToResourceFromEntity(PrecioHistorico entity)
    {
        return new PrecioHistoricoResource(entity.FechaEmision, entity.PrecioUnitario);
    }
}