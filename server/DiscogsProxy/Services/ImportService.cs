using DiscogsProxy.DTO;
using DiscogsProxy.Workers;
using Microsoft.EntityFrameworkCore;

namespace DiscogsProxy.Services;

/// <summary>
/// Import Service
/// </summary>
/// <param name="context"></param>
public class ImportService(DiscogsContext context, IDiscogsApiHelper apiHelper, IConfiguration config, ICollectionService collectionService, IWantListService wantListService) : IImportService
{
    private readonly DiscogsContext _context = context;
    private readonly IDiscogsApiHelper _apiHelper = apiHelper;
    private readonly IConfiguration _config = config;
    private readonly ICollectionService _collectionService = collectionService;
    private readonly IWantListService _wantlistService = wantListService;

    /// <summary>
    /// Import data from Discogs
    /// This will clear the existing data and re-import the collection and wantlist
    /// </summary>
    /// <returns></returns>
    public async Task<ResultObject<bool>> ImportData()
    {
        var result = new ResultObject<bool>();
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            if (!await _apiHelper.ProfileIsValid())
            {
                result.Error = new Exception("Invalid profile configuration");
                return result;
            }

            // Clear existing data (see next step)
            await TruncateTablesAsync();

            // Import collection, genres and styles
            var collectionImport = await ImportCollection();
            if (collectionImport.HasError)
            {
                await transaction.RollbackAsync();
                return collectionImport;
            }

            // import wantlist
            var wantlistImport = await ImportWantlist();
            if (wantlistImport.HasError)
            {
                await transaction.RollbackAsync();
                return collectionImport;
            }

            // Commit if all successful
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            // Rollback if anything fails
            await transaction.RollbackAsync();
            result.Error = ex;
        }

        return result;
    }

    /// <summary>
    /// Truncate the tables in the database
    /// This will delete all data and reset identity counters
    /// </summary>   
    public async Task TruncateTablesAsync()
    {
        var tableNames = new[] { "Collection", "Wantlist", "Styles", "Genres" };

        foreach (var table in tableNames)
        {
            await _context.Database.ExecuteSqlAsync($"DELETE FROM [{table}]");
        }

        // Reset identity counters
        foreach (var table in tableNames)
        {
            await _context.Database.ExecuteSqlAsync($"DELETE FROM sqlite_sequence WHERE name = '{table}'");
        }
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
    /// Import data from Discogs
    /// This will clear the existing data and re-import the collection and wantlist
    /// </summary>
    /// <returns></returns>
    Task<ResultObject<bool>> ImportData();
}