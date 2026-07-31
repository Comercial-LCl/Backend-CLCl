// Invoicing/Infrastructure/Persistence/EntityFrameworkCore/Configuration/Extensions/ModelBuilderExtensions.cs
using Microsoft.EntityFrameworkCore;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Entities;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyInvoicingConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Proveedor>(entity =>
        {
            entity.ToTable("proveedores");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Id).IsRequired().ValueGeneratedNever();
            entity.OwnsOne(p => p.Ruc, ruc =>
            {
                ruc.Property(r => r.Valor).HasColumnName("ruc").IsRequired().HasMaxLength(11);
                ruc.HasIndex(r => r.Valor).IsUnique();
            });
            entity.Property(p => p.RazonSocial).IsRequired().HasMaxLength(255);
        });

        builder.Entity<Categoria>(entity =>
        {
            entity.ToTable("categorias");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).IsRequired().ValueGeneratedNever();
            entity.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            entity.HasIndex(c => c.Nombre).IsUnique();
        });

        builder.Entity<Factura>(entity =>
        {
            entity.ToTable("facturas");
            entity.HasKey(f => f.Id);
            entity.Property(f => f.Id).IsRequired().ValueGeneratedNever();
            entity.Property(f => f.UsuarioId).IsRequired();
            entity.Property(f => f.ProveedorId).IsRequired();
            entity.Property(f => f.CategoriaId); // ya no .IsRequired()
            entity.Property(f => f.Tipo).IsRequired().HasConversion<string>().HasMaxLength(20);
            entity.Property(f => f.FechaEmision).IsRequired().HasColumnType("timestamp without time zone");
            entity.Property(f => f.ResumenIa).HasMaxLength(2000);
            entity.Property(f => f.ArchivoUrl); // ya no .IsRequired()
            entity.Property(f => f.EstadoProcesamiento).IsRequired().HasConversion<string>().HasMaxLength(20);
            entity.Property(f => f.CreatedAt).IsRequired();

            entity.OwnsOne(f => f.NumeroFactura, nf =>
            {
                nf.Property(n => n.Serie).HasColumnName("serie").IsRequired().HasMaxLength(10);
                nf.Property(n => n.Numero).HasColumnName("numero").IsRequired().HasMaxLength(20);
            });

            entity.OwnsOne(f => f.Monto, m =>
            {
                m.Property(x => x.Valor).HasColumnName("monto_total").IsRequired().HasColumnType("numeric(12,2)");
                m.Property(x => x.Moneda).HasColumnName("moneda").IsRequired().HasMaxLength(3);
            });

            entity.HasMany(f => f.Items)
                .WithOne()
                .HasForeignKey(i => i.FacturaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(f => f.Items).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        builder.Entity<ItemFactura>(entity =>
        {
            entity.ToTable("item_facturas");
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Id).IsRequired().ValueGeneratedNever();
            entity.Property(i => i.Descripcion).IsRequired().HasMaxLength(255);
            entity.Property(i => i.Cantidad).IsRequired().HasColumnType("numeric(10,2)");
            entity.Property(i => i.PrecioUnitario).IsRequired().HasColumnType("numeric(12,2)");
            entity.Property(i => i.Subtotal).IsRequired().HasColumnType("numeric(12,2)");
        });
    }
}