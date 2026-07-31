// Iam/Domain/Model/Aggregates/Usuario.cs
using System.Text.Json.Serialization;

namespace FacturasIA.Platform.Iam.Domain.Model.Aggregates;

/// <summary>
///     Usuario aggregate root — dueño de negocio que usa el sistema.
/// </summary>
public class Usuario(string nombre, string email, string passwordHash, string? rucNegocio = null)
{
    public Usuario() : this(string.Empty, string.Empty, string.Empty)
    {
    }

    public Guid Id { get; } = Guid.NewGuid();
    public string Nombre { get; private set; } = nombre;
    public string Email { get; private set; } = email;

    [JsonIgnore] public string PasswordHash { get; private set; } = passwordHash;

    public string? RucNegocio { get; private set; } = rucNegocio;

    public Usuario UpdatePasswordHash(string passwordHash)
    {
        PasswordHash = passwordHash;
        return this;
    }

    public Usuario UpdateRucNegocio(string? rucNegocio)
    {
        RucNegocio = rucNegocio;
        return this;
    }
}