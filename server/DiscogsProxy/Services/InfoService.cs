using DiscogsProxy.DTO;
using DiscogsProxy.Workers;
using Microsoft.EntityFrameworkCore;

namespace DiscogsProxy.Services;

/// <summary>
/// Service to query the DB and get underlying data
/// </summary>
/// <param name="discogsContext"></param>
/// <param name="dbChecker"></param>
public class InfoService(DiscogsContext discogsContext, IDatabaseChecker dbChecker) : IInfoService
{
    private readonly DiscogsContext _discogsContext = discogsContext;
    private readonly IDatabaseChecker _dbChecker = dbChecker;

    /// <summary>
    /// Get all artists in the collection
    /// </summary>
    /// <returns></returns>
    public ResultObject<List<string>> GetArtists()
    {
        var result = new ResultObject<List<string>>();

        if (!_dbChecker.CanConnect())
        {
            result.Error = new Exception("Cannot connect to Database");
            return result;
        }

        // ToList in the middle here is important to load the items into memory to then run SelectManyOn
        // SQL APPLY not supported on SQL Lite
        var artistNames = _discogsContext.Collection.AsNoTracking().ToList().SelectMany(x => x.ArtistName!).Distinct().Order();

        result.Result = [.. artistNames];
        return result;
    }

    /// <summary>
    /// Get all genres in the collection
    /// </summary>
    /// <returns></returns>
    public ResultObject<List<string>> GetGenres()
    {
        var result = new ResultObject<List<string>>();

        if (!_dbChecker.CanConnect())
        {
            result.Error = new Exception("Cannot connect to Database");
            return result;
        }

        var genres = _discogsContext.Genres.AsNoTracking().Select(x => x.Text!).Order();

        result.Result = [.. genres];
        return result;
    }

    /// <summary>
    /// Get all styles in the collection
    /// </summary>
    /// <returns></returns>
    public ResultObject<List<string>> GetStyles()
    {
        var result = new ResultObject<List<string>>();

        if (!_dbChecker.CanConnect())
        {
            result.Error = new Exception("Cannot connect to Database");
            return result;
        }

        var styles = _discogsContext.Styles.AsNoTracking().Select(x => x.Text!).Order();

        result.Result = [.. styles];
        return result;
    }
}

/// <summary>
/// Interface
/// </summary>
public interface IInfoService
{
    /// <summary>
    /// Get all artists in the collection
    /// </summary>
    /// <returns></returns>
    ResultObject<List<string>> GetArtists();

    /// <summary>
    /// Get all genres in the collection
    /// </summary>
    /// <returns></returns>
    ResultObject<List<string>> GetStyles();

    /// <summary>
    /// Get all styles in the collection
    /// </summary>
    /// <returns></returns>
    ResultObject<List<string>> GetGenres();
}