using System.Text.Json.Nodes;
using DiscogsProxy.DTO;
using DiscogsProxy.Workers;
using Microsoft.EntityFrameworkCore;

namespace DiscogsProxy.Services;

/// <summary>
/// Collection Service
/// </summary>
/// <param name="context"></param>
public class CollectionService(DiscogsContext context, IDiscogsApiHelper apiHelper) : ICollectionService
{
    private readonly DiscogsContext _context = context;
    private readonly IDiscogsApiHelper _apiHelper = apiHelper;

    /// <summary>
    /// Get a random item from the collection
    /// </summary>
    /// <returns></returns>
    public async Task<ResultObject<CollectionItem>> GetRandomCollectionItem()
    {
        var result = new ResultObject<CollectionItem>();

        if (_context.Collection.Count() == 0)
        {
            result.Error = new Exception("No collection available");
            return result;
        }

        int count = await _context.Collection.CountAsync();

        int index = new Random().Next(count);

        // Get a random value
        result.Result = await _context.Collection
            .Skip(index)
            .FirstOrDefaultAsync();

        return result;
    }

    /// <summary>
    /// Call the Discogs API and get all items in the user's collection
    /// </summary>
    /// <param name="username"></param>
    /// <param name="page"></param>
    /// <param name="perPage"></param>
    /// <returns></returns>
    public async Task<ResultObject<List<CollectionItem>>> GetCollection(string username, int page, int perPage)
    {
        var result = new ResultObject<List<CollectionItem>>();
        var client = _apiHelper.CreateClient();

        // We can call the api with some default params
        // And then use the pagination info from this response to decide if we have to make more calls
        var initialPage = await GetCollectionPage(client, username, page, perPage);

        if (initialPage.HasError)
        {
            result.Error = initialPage.Error;
            return result;
        }

        var obj = await _apiHelper.GetJsonObjectFromResponseAsync(initialPage!.Result!);

        var allReleases = new JsonArray();

        // Get all items that exist within the "releases" property
        // And add them to allItems
        _apiHelper.GetEntriesFromPage(allReleases, obj!, "releases");

        // Now we can read the pagination info, and see if we need to call the api again 
        await _apiHelper.CheckAllPages(obj!, client, username, page, perPage, GetCollectionPage, allReleases, "releases");

        var mappedReleases = _apiHelper.MapReleases<CollectionItem>(allReleases);

        return mappedReleases;
    }

    /// <summary>
    /// Query the Discogs API for the given user's collection
    /// </summary>
    /// <param name="client"></param>
    /// <param name="username"></param>
    /// <param name="page"></param>
    /// <param name="perPage"></param>
    /// <returns></returns>
    private static async Task<ResultObject<HttpResponseMessage>> GetCollectionPage(HttpClient client, string username, int page, int perPage)
    {
        var result = new ResultObject<HttpResponseMessage>();
        var response = await client.GetAsync($"users/{username}/collection/folders/0/releases?page={page}&per_page={perPage}");

        if (!response.IsSuccessStatusCode)
        {
            result.Error = new Exception("Error from Discogs collection API");
            return result;
        }

        result.Result = response;
        return result;
    }
}

/// <summary>
/// Interface for collection service
/// </summary>
public interface ICollectionService
{
    /// <summary>
    /// Get a random item from the collection
    /// </summary>
    /// <returns></returns>
    Task<ResultObject<CollectionItem>> GetRandomCollectionItem();

    /// <summary>
    /// Call the Discogs API and get all items in the user's collection
    /// </summary>
    /// <param name="username"></param>
    /// <param name="page"></param>
    /// <param name="perPage"></param>
    /// <returns></returns>
    Task<ResultObject<List<CollectionItem>>> GetCollection(string username, int page, int perPage);
}