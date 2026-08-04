// Invoicing/Domain/Model/Queries/ObtenerHistorialPreciosQuery.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.Queries;

public record ObtenerHistorialPreciosQuery(Guid ProductoId, Guid UsuarioId);