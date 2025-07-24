using DiscogsProxy.DTO;
using DiscogsProxy.Workers;
using Microsoft.EntityFrameworkCore;

namespace DiscogsProxy.Services;

public class InfoService(DiscogsContext discogsContext, IDatabaseChecker dbChecker) : IInfoService
{
    private readonly DiscogsContext _discogsContext = discogsContext;
    private readonly IDatabaseChecker _dbChecker = dbChecker;

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

public interface IInfoService
{

    ResultObject<List<string>> GetArtists();

    ResultObject<List<string>> GetStyles();

    ResultObject<List<string>> GetGenres();
}