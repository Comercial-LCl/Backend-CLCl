// Invoicing/Interfaces/Rest/Resources/ItemFacturaResource.cs
namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

public record ItemFacturaResource(Guid Id, string Descripcion, decimal Cantidad, decimal PrecioUnitario, decimal Subtotal);