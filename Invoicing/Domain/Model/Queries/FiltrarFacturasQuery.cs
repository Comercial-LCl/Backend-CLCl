// Invoicing/Domain/Model/Queries/FiltrarFacturasQuery.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.Queries;

public record FiltrarFacturasQuery(
    Guid UsuarioId,
    Guid? ProveedorId,
    Guid? CategoriaId,
    DateTime? Desde,
    DateTime? Hasta);