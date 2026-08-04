// Invoicing/Interfaces/Rest/Resources/ProductoResource.cs
namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

public record ProductoResource(Guid Id, Guid ProveedorId, string Nombre);