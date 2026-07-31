// Invoicing/Infrastructure/Persistence/EntityFrameworkCore/Repositories/CategoriaRepository.cs
using Microsoft.EntityFrameworkCore;
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Repositories;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class CategoriaRepository(AppDbContext context) : BaseRepository<Categoria>(context), ICategoriaRepository
{
    public async Task<Categoria?> FindByNombreAsync(string nombre, CancellationToken cancellationToken)
    {
        return await Context.Set<Categoria>().FirstOrDefaultAsync(c => c.Nombre == nombre, cancellationToken);
    }
}