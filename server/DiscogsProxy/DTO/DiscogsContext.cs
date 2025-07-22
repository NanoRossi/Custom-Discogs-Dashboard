using Microsoft.EntityFrameworkCore;

namespace DiscogsProxy.DTO;

public class DiscogsContext : DbContext
{
    public DiscogsContext() : base()
    {

    }

    public DiscogsContext(DbContextOptions<DiscogsContext> options) : base(options)
    {

    }

    public virtual DbSet<CollectionItem> Collection { get; set; } = default!;

    public virtual DbSet<WantlistItem> Wantlist { get; set; } = default!;
}
