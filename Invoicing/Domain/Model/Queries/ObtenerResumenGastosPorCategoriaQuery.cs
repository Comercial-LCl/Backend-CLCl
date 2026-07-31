// Invoicing/Domain/Model/Queries/ObtenerResumenGastosPorCategoriaQuery.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.Queries;

public record ObtenerResumenGastosPorCategoriaQuery(Guid UsuarioId, DateTime? Desde, DateTime? Hasta);