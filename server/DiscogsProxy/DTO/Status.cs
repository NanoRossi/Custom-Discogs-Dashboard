using DiscogsProxy.Constants;

namespace DiscogsProxy.DTO;

public class Status
{
    public Status(string dbStatus)
    {
        this.DatabaseStatus = dbStatus;
    }

    public Status(DiscogsContext context)
    {
        this.CollectionCount = context.Collection.Count();
        this.WantlistCount = context.Wantlist.Count();
        this.GenreCount = context.Genres.Count();
        this.StyleCount = context.Styles.Count();
        this.VinylCount = context.Collection.Count(x => x.FormatInfo!.FormatType == "Vinyl");
        this.CDCount = context.Collection.Count(x => x.FormatInfo!.FormatType == "CD");
        this.CassetteCount = context.Collection?.Count(x => x.FormatInfo!.FormatType == "Cassette");
        this.DatabaseStatus = (CollectionCount > 0 && WantlistCount > 0) ? DbStatus.Active : DbStatus.Empty;
    }

    public string? DatabaseStatus { get; set; }

    public int? CollectionCount { get; set; }

    public int? WantlistCount { get; set; }

    public int? GenreCount { get; set; }

    public int? StyleCount { get; set; }

    public int? VinylCount { get; set; }

    public int? CDCount { get; set; }

    public int? CassetteCount { get; set; }
}