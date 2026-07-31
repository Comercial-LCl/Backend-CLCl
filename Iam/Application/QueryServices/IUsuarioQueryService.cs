// Iam/Application/QueryServices/IUsuarioQueryService.cs
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;
using FacturasIA.Platform.Iam.Domain.Model.Queries;

namespace FacturasIA.Platform.Iam.Application.QueryServices;

public interface IUsuarioQueryService
{
    Task<Usuario?> Handle(GetUsuarioByIdQuery query, CancellationToken cancellationToken);
    Task<Usuario?> Handle(GetUsuarioByEmailQuery query, CancellationToken cancellationToken);
}