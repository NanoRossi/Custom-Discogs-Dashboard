using DiscogsProxy.Constants;
using DiscogsProxy.DTO;

namespace DiscogsProxy.Services;

/// <summary>
/// Status Service
/// </summary>
/// <param name="context"></param>
public class StatusService(DiscogsContext context) : IStatusService
{
    private readonly DiscogsContext _context = context;

    /// <summary>
    /// Get the current Db information
    /// </summary>
    /// <returns></returns>
    public ResultObject<Status> GetStatus()
    {
        var result = new ResultObject<Status>();

        if (!_context.Database.CanConnect())
        {
            result.Result = new Status(DbStatus.Disconnected);
            return result;
        }

        result.Result = new Status(_context);

        return result;
    }
}

/// <summary>
/// Interface for status service
/// </summary>
public interface IStatusService
{
    /// <summary>
    /// Get the current Db information
    /// </summary>
    /// <returns></returns>
    ResultObject<Status> GetStatus();
}