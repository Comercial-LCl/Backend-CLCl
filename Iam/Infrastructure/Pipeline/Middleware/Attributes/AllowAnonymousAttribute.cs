// Iam/Infrastructure/Pipeline/Middleware/Attributes/AllowAnonymousAttribute.cs
namespace FacturasIA.Platform.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Marca una acción que no requiere autorización.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute;