// Iam/Domain/Model/Commands/SignUpCommand.cs
namespace FacturasIA.Platform.Iam.Domain.Model.Commands;

/// <param name="Nombre">Nombre del dueño del negocio</param>
/// <param name="Email">Email usado para login</param>
/// <param name="Password">Contraseña en texto plano, se hashea antes de guardar</param>
/// <param name="RucNegocio">RUC del negocio (opcional)</param>
public record SignUpCommand(string Nombre, string Email, string Password, string? RucNegocio);