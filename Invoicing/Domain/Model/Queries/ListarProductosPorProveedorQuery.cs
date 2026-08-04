// Invoicing/Domain/Model/Queries/ListarProductosPorProveedorQuery.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.Queries;

public record ListarProductosPorProveedorQuery(Guid ProveedorId, Guid UsuarioId);