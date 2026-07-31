// Invoicing/Infrastructure/Persistence/EntityFrameworkCore/Repositories/ProveedorRepository.cs
using Microsoft.EntityFrameworkCore;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Repositories;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class ProveedorRepository(AppDbContext context) : BaseRepository<Proveedor>(context), IProveedorRepository
{
    public async Task<Proveedor?> FindByRucAsync(string ruc, CancellationToken cancellationToken)
    {
        return await Context.Set<Proveedor>().FirstOrDefaultAsync(p => p.Ruc.Valor == ruc, cancellationToken);
    }
}