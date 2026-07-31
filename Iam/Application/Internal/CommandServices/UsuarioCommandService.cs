// Iam/Application/Internal/CommandServices/UsuarioCommandService.cs
using Microsoft.EntityFrameworkCore;
using FacturasIA.Platform.Iam.Application.CommandServices;
using FacturasIA.Platform.Iam.Application.Internal.OutboundServices;
using FacturasIA.Platform.Iam.Domain.Model;
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;
using FacturasIA.Platform.Iam.Domain.Model.Commands;
using FacturasIA.Platform.Iam.Domain.Repositories;
using FacturasIA.Platform.Shared.Application.Model;
using FacturasIA.Platform.Shared.Domain.Repositories;

namespace FacturasIA.Platform.Iam.Application.Internal.CommandServices;

/// <summary>
///     Usuario command service
/// </summary>
/// <param name="usuarioRepository">Usuario repository</param>
/// <param name="tokenService">Token service</param>
/// <param name="hashingService">Hashing service</param>
/// <param name="unitOfWork">Unit of work</param>
public class UsuarioCommandService(
    IUsuarioRepository usuarioRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork)
    : IUsuarioCommandService
{
    /// <inheritdoc />
    public async Task<Result> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        if (await usuarioRepository.ExistsByEmailAsync(command.Email, cancellationToken))
            return Result.Failure(IamError.EmailAlreadyTaken, "Ya existe una cuenta registrada con ese email.");

        var hashedPassword = hashingService.HashPassword(command.Password);
        var usuario = new Usuario(command.Nombre, command.Email, hashedPassword, command.RucNegocio);

        try
        {
            await usuarioRepository.AddAsync(usuario, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result.Success();
        }
        catch (OperationCanceledException)
        {
            return Result.Failure(IamError.OperationCancelled, "La operación fue cancelada.");
        }
        catch (DbUpdateException)
        {
            return Result.Failure(IamError.DatabaseError, "Ocurrió un error al guardar el usuario.");
        }
        catch (Exception)
        {
            return Result.Failure(IamError.InternalServerError, "Ocurrió un error inesperado.");
        }
    }

    /// <inheritdoc />
    public async Task<Result<(Usuario Usuario, string Token)>> Handle(SignInCommand command,
        CancellationToken cancellationToken)
    {
        var usuario = await usuarioRepository.FindByEmailAsync(command.Email, cancellationToken);

        if (usuario is null || !hashingService.VerifyPassword(command.Password, usuario.PasswordHash))
            return Result<(Usuario Usuario, string Token)>.Failure(IamError.InvalidCredentials,
                "Email o contraseña incorrectos.");

        var token = tokenService.GenerateToken(usuario);
        return Result<(Usuario Usuario, string Token)>.Success((usuario, token));
    }
}