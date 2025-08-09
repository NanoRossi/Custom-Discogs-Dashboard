using DiscogsProxy.Services;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WantlistItem>(entity =>
        {
            entity.HasKey(ci => ci.ReleaseKey);

            // FormatInfo is owned by WantlistItem
            entity.OwnsOne(ci => ci.FormatInfo, fmt =>
            {
                fmt.Property(f => f.FormatType);

                // DiscInfo is a collection owned by FormatInfo
                fmt.OwnsMany(f => f.DiscInfo, disc =>
                {
                    disc.Property(d => d.Quantity);
                    disc.Property(d => d.Text);

                    // You may need a key here (required for Owned Collections in EF Core)
                    disc.WithOwner();
                    disc.HasKey("Id"); // Shadow key
                });
            });
        });

        modelBuilder.Entity<CollectionItem>(entity =>
        {
            entity.HasKey(ci => ci.ReleaseKey);

            // FormatInfo is owned by CollectionItem
            entity.OwnsOne(ci => ci.FormatInfo, fmt =>
            {
                fmt.Property(f => f.FormatType);

                // DiscInfo is a collection owned by FormatInfo
                fmt.OwnsMany(f => f.DiscInfo, disc =>
                {
                    disc.Property(d => d.Quantity);
                    disc.Property(d => d.Text);

                    // You may need a key here (required for Owned Collections in EF Core)
                    disc.WithOwner();
                    disc.HasKey("Id"); // Shadow key
                });
            });
        });
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
