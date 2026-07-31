// Invoicing/Interfaces/Rest/Resources/ResumenGastosPorCategoriaResource.cs
namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

public record ResumenGastosPorCategoriaResource(Guid? CategoriaId, string CategoriaNombre, decimal Total);