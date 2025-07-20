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
        this.DatabaseStatus = (CollectionCount > 0 && WantlistCount > 0) ? DbStatus.Active : DbStatus.Empty;
    }

    public string? DatabaseStatus { get; set; }

    public int? CollectionCount { get; set; }

    public int? WantlistCount { get; set; }
}