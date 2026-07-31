// Shared/Infrastructure/Persistence/EntityFrameworkCore/Configuration/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using FacturasIA.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;
using FacturasIA.Platform.Invoicing.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

namespace FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Application database context.
/// </summary>
/// <remarks>
///     Cada bounded context registra su propia configuración de entidades vía un método de
///     extensión sobre ModelBuilder, invocado desde OnModelCreating.
/// </remarks>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyIamConfiguration();
        builder.ApplyInvoicingConfiguration();
    }
}