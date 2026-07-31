// Iam/Application/Internal/QueryServices/UsuarioQueryService.cs
using FacturasIA.Platform.Iam.Application.QueryServices;
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;
using FacturasIA.Platform.Iam.Domain.Model.Queries;
using FacturasIA.Platform.Iam.Domain.Repositories;

namespace FacturasIA.Platform.Iam.Application.Internal.QueryServices;

public class UsuarioQueryService(IUsuarioRepository usuarioRepository) : IUsuarioQueryService
{
    public async Task<Usuario?> Handle(GetUsuarioByIdQuery query, CancellationToken cancellationToken)
    {
        return await usuarioRepository.FindByIdAsync(query.UsuarioId, cancellationToken);
    }

    public async Task<Usuario?> Handle(GetUsuarioByEmailQuery query, CancellationToken cancellationToken)
    {
        return await usuarioRepository.FindByEmailAsync(query.Email, cancellationToken);
    }
}