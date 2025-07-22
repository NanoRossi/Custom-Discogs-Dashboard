using System.Text.Json.Nodes;
using DiscogsProxy.DTO;
using DiscogsProxy.Workers;

namespace DiscogsProxy.Services;

/// <summary>
/// Wantlist Service
/// </summary>
/// <param name="context"></param>
public class WantListService(DiscogsContext context, IDiscogsApiHelper apiHelper) : IWantListService
{
    private readonly DiscogsContext _context = context;
    private readonly IDiscogsApiHelper _apiHelper = apiHelper;

    /// <summary>
    /// Get all items in the wantlist
    /// </summary>
    /// <returns></returns>
    public ResultObject<List<WantlistItem>> GetWantlistItems()
    {
        var result = new ResultObject<List<WantlistItem>>();

        if (!_context.Wantlist.Any())
        {
            result.Error = new Exception("No wantlist available");
            return result;
        }

        result.Result = [.. _context.Wantlist];
        return result;
    }

    /// <summary>
    /// Query Discogs to get all Wantlist items for the given user
    /// </summary>
    /// <param name="username"></param>
    /// <param name="page"></param>
    /// <param name="perPage"></param>
    /// <returns></returns>
    public async Task<ResultObject<List<WantlistItem>>> GetWantList(string username, int page, int perPage)
    {
        var result = new ResultObject<List<WantlistItem>>();
        var client = _apiHelper.CreateClient();

        // We can call the api with some default params
        // And then use the pagination info from this response to decide if we have to make more calls
        var initialPage = await GetWantlistPage(client, username, page, perPage);

        if (initialPage.HasError)
        {
            result.Error = initialPage.Error;
            return result;
        }

        var jsonObj = await _apiHelper.GetJsonObjectFromResponseAsync(initialPage!.Result!);

        var allItems = new JsonArray();

        // Get all items that exist within the "wants" property
        // And add them to allItems
        _apiHelper.GetEntriesFromPage(allItems, jsonObj!, "wants");

        // Now we can read the pagination info, and see if we need to call the api again 
        await _apiHelper.CheckAllPages(jsonObj!, client, username, page, perPage, GetWantlistPage, allItems, "wants");

        var mappedReleases = _apiHelper.MapReleases<WantlistItem>(allItems);

        return mappedReleases;
    }

    /// <summary>
    /// Query the Discogs wantlist API for the given user
    /// </summary>
    /// <param name="client"></param>
    /// <param name="username"></param>
    /// <param name="page"></param>
    /// <param name="perPage"></param>
    /// <returns></returns>
    public async Task<ResultObject<HttpResponseMessage>> GetWantlistPage(HttpClient client, string username, int page, int perPage)
    {
        var result = new ResultObject<HttpResponseMessage>();

        // Discogs wantlist API
        var response = await client.GetAsync($"users/{username}/wants?page={page}&per_page={perPage}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            result.Error = new Exception($"Error from Discogs collection API: {errorContent}");
            return result;
        }

        result.Result = response;
        return result;
    }
}

/// <summary>
/// Interface for wantlist service
/// </summary>
public interface IWantListService
{
    /// <summary>
    /// Get all items in the wantlist
    /// </summary>
    /// <returns></returns>
    ResultObject<List<WantlistItem>> GetWantlistItems();

    /// <summary>
    /// Query Discogs to get all Wantlist items for the given user
    /// </summary>
    /// <param name="username"></param>
    /// <param name="page"></param>
    /// <param name="perPage"></param>
    /// <returns></returns>
    Task<ResultObject<List<WantlistItem>>> GetWantList(string username, int page, int perPage);
}