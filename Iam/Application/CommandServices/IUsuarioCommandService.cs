// Iam/Application/CommandServices/IUsuarioCommandService.cs
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;
using FacturasIA.Platform.Iam.Domain.Model.Commands;
using FacturasIA.Platform.Shared.Application.Model;

namespace FacturasIA.Platform.Iam.Application.CommandServices;

public interface IUsuarioCommandService
{
    Task<Result> Handle(SignUpCommand command, CancellationToken cancellationToken);

    Task<Result<(Usuario Usuario, string Token)>> Handle(SignInCommand command, CancellationToken cancellationToken);
}