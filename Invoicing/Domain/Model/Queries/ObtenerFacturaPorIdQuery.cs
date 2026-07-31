// Invoicing/Domain/Model/Queries/ObtenerFacturaPorIdQuery.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.Queries;

public record ObtenerFacturaPorIdQuery(Guid FacturaId, Guid UsuarioId);