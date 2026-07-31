// Invoicing/Interfaces/Rest/Transform/FacturaResourceFromEntityAssembler.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Transform;

public static class FacturaResourceFromEntityAssembler
{
    public static FacturaResource ToResourceFromEntity(Factura entity)
    {
        return new FacturaResource(
            entity.Id,
            entity.ProveedorId,
            entity.CategoriaId,
            entity.Tipo.ToString(),
            entity.NumeroFactura.Serie,
            entity.NumeroFactura.Numero,
            entity.FechaEmision,
            entity.Monto.Valor,
            entity.Monto.Moneda,
            entity.ResumenIa,
            entity.ArchivoUrl,
            entity.EstadoProcesamiento.ToString(),
            entity.Items.Select(i => new ItemFacturaResource(i.Id, i.Descripcion, i.Cantidad, i.PrecioUnitario, i.Subtotal)));
    }
}