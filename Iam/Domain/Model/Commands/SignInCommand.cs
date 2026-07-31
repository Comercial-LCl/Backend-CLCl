// Iam/Domain/Model/Commands/SignInCommand.cs
namespace FacturasIA.Platform.Iam.Domain.Model.Commands;

public record SignInCommand(string Email, string Password);