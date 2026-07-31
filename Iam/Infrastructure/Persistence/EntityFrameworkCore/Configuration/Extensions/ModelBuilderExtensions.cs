// Iam/Infrastructure/Persistence/EntityFrameworkCore/Configuration/Extensions/ModelBuilderExtensions.cs
using Microsoft.EntityFrameworkCore;
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;

namespace FacturasIA.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyIamConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Usuario>().ToTable("usuarios");
        builder.Entity<Usuario>().HasKey(u => u.Id);
        builder.Entity<Usuario>().Property(u => u.Id).IsRequired().ValueGeneratedNever();
        builder.Entity<Usuario>().Property(u => u.Nombre).IsRequired().HasMaxLength(150);
        builder.Entity<Usuario>().Property(u => u.Email).IsRequired().HasMaxLength(255);
        builder.Entity<Usuario>().Property(u => u.PasswordHash).IsRequired();
        builder.Entity<Usuario>().Property(u => u.RucNegocio).HasMaxLength(11);
        builder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
    }
}