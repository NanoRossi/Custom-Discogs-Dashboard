using DiscogsProxy.DTO;
using Microsoft.EntityFrameworkCore;

namespace DiscogsProxy.DTO;

public class DiscogsContext(DbContextOptions<DiscogsContext> options) : DbContext(options)
{
    public DbSet<CollectionItem> Collection { get; set; } = default!;

    public DbSet<WantlistItem> Wantlist { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
