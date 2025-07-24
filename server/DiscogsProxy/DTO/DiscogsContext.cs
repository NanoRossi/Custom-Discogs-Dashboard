using Microsoft.EntityFrameworkCore;

namespace DiscogsProxy.DTO;

public class DiscogsContext : DbContext
{
    /// <summary>
    /// Parameterless Contructor - allows mocking in unit tests
    /// </summary>
    public DiscogsContext() : base()
    {

    }

    /// <summary>
    /// Default constructor when running app
    /// </summary>
    /// <param name="options"></param>
    public DiscogsContext(DbContextOptions<DiscogsContext> options) : base(options)
    {

    }

    /// <summary>
    /// Collection
    /// </summary>
    public virtual DbSet<CollectionItem> Collection { get; set; } = default!;

    /// <summary>
    /// Wantlist
    /// </summary>
    public virtual DbSet<WantlistItem> Wantlist { get; set; } = default!;

    public virtual DbSet<MusicStyle> Styles { get; set; } = default!;

    public virtual DbSet<MusicGenre> Genres { get; set; } = default!;
}
