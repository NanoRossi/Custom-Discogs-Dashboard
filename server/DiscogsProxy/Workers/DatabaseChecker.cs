using DiscogsProxy.DTO;

namespace DiscogsProxy.Workers;

/// <summary>
/// Helper class to check connection to DB
/// </summary>
/// <remarks>
/// Constructor
/// </remarks>
/// <param name="context"></param>
public class DatabaseChecker(DiscogsContext context) : IDatabaseChecker
{
    private readonly DiscogsContext _context = context;

    /// <summary>
    /// Helper func to abstract the .Database.CanConnect() method
    /// Purely to help with mocking for unit tetss
    /// As .Database is virtual, and mocking it gets messy
    /// </summary>
    /// <returns></returns>
    public bool CanConnect() => _context.Database.CanConnect();
}

/// <summary>
/// Interface for DatabaseChecker
/// </summary>
public interface IDatabaseChecker
{
    /// <summary>
    /// Helper func to abstract the .Database.CanConnect() method
    /// Purely to help with mocking for unit tetss
    /// As .Database is virtual, and mocking it gets messy
    /// </summary>
    /// <returns></returns>
    bool CanConnect();
}