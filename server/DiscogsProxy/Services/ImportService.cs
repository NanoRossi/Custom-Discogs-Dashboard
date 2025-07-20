using DiscogsProxy.Constants;
using DiscogsProxy.DTO;
using DiscogsProxy.Workers;

namespace DiscogsProxy.Services;

/// <summary>
/// Import Service
/// </summary>
/// <param name="context"></param>
public class ImportService(DiscogsContext context, IConfiguration config, ICollectionService collectionService, IWantListService wantListService) : IImportService
{
    private readonly DiscogsContext _context = context;
    private readonly IConfiguration _config = config;
    private readonly ICollectionService _collectionService = collectionService;
    private readonly IWantListService _wantlistService = wantListService;

    /// <summary>
    /// Recycle the DB
    /// Delete everything and re-build tables from scratch 
    /// </summary>
    public async void RecycleDb()
    {
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Build the collection table by querying Discogs
    /// </summary>
    /// <returns></returns>
    public async Task<ResultObject<bool>> ImportCollection()
    {
        var result = new ResultObject<bool>();
        var username = _config["DiscogsUsername"];

        if (string.IsNullOrEmpty(username))
        {
            result.Error = new Exception("Invalid Username");
            return result;
        }

        // We'll use 200 per page, it's a decent size
        // And start from page 1. The func will then call any other pages iteratively
        var getCollection = await _collectionService.GetCollection(username, 1, 200);

        if (getCollection.HasError)
        {
            result.Error = getCollection.Error;
            return result;
        }

        _context.Collection.AddRange(getCollection!.Result!);
        await _context.SaveChangesAsync();

        return result;
    }

    /// <summary>
    /// Build the wantlist table by querying Discogs
    /// </summary>
    /// <returns></returns>
    public async Task<ResultObject<bool>> ImportWantlist()
    {
        var result = new ResultObject<bool>();
        var username = _config["DiscogsUsername"];

        if (string.IsNullOrEmpty(username))
        {
            result.Error = new Exception("Invalid Username");
            return result;
        }

        var getWantlist = await _wantlistService.GetWantList(username, 1, 200);

        if (getWantlist.HasError)
        {
            result.Error = getWantlist.Error;
            return result;
        }

        _context.Wantlist.AddRange(getWantlist!.Result!);
        await _context.SaveChangesAsync();

        return result;
    }
}

/// <summary>
/// Interface for import service
/// </summary>
public interface IImportService
{
    /// <summary>
    /// Recycle the DB
    /// Delete everything and re-build tables from scratch 
    /// </summary>
    void RecycleDb();

    /// <summary>
    /// Build the collection table by querying Discogs
    /// </summary>
    /// <returns></returns>
    Task<ResultObject<bool>> ImportCollection();

    /// <summary>
    /// Build the wantlist table by querying Discogs
    /// </summary>
    /// <returns></returns>
    Task<ResultObject<bool>> ImportWantlist();
}