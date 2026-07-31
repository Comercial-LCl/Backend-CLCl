// Invoicing/Domain/Model/Queries/ObtenerResumenGastosPorPeriodoQuery.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.Queries;

public record ObtenerResumenGastosPorPeriodoQuery(Guid UsuarioId, DateTime Desde, DateTime Hasta);