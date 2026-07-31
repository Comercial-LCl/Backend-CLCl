// Iam/Domain/Model/IamError.cs
namespace FacturasIA.Platform.Iam.Domain.Model;

public enum IamError
{
    None,
    InvalidCredentials,
    EmailAlreadyTaken,
    UsuarioNotFound,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}