// Iam/Infrastructure/Persistence/EntityFrameworkCore/Repositories/UsuarioRepository.cs
using Microsoft.EntityFrameworkCore;
using FacturasIA.Platform.Iam.Domain.Model.Aggregates;
using FacturasIA.Platform.Iam.Domain.Repositories;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace FacturasIA.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class UsuarioRepository(AppDbContext context)
    : BaseRepository<Usuario>(context), IUsuarioRepository
{
    public async Task<Usuario?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await Context.Set<Usuario>().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await Context.Set<Usuario>().AnyAsync(u => u.Email == email, cancellationToken);
    }
}