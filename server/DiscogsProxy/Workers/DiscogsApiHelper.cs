using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using DiscogsProxy.DTO;

namespace DiscogsProxy.Workers;

/// <summary>
/// Helper methods to handle API responses from Discogs
/// </summary>
/// <param name="httpClientFactory"></param>
/// <param name="config"></param>
public class DiscogsApiHelper(IHttpClientFactory httpClientFactory, IConfiguration config) : IDiscogsApiHelper
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly string _userAgent = config["UserAgent"] ?? throw new ArgumentNullException("UserAgent env variable not found");
    private readonly string _token = config["DiscogsToken"] ?? throw new ArgumentNullException("DiscogsToken env variable not found");

    /// <summary>
    /// Create a HTTP Client to Discogs using our token
    /// </summary>
    /// <returns></returns>
    public HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://api.discogs.com/");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Discogs", $"token={_token}");
        client.DefaultRequestHeaders.UserAgent.ParseAdd($"DiscogsApiApp/1.0 ({_userAgent})");
        return client;
    }

    /// <summary>
    /// Get the json content from a HttpResponseMessage
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    public async Task<JsonObject> GetJsonObjectFromResponseAsync(HttpResponseMessage response)
    {
        var stream = await response.Content.ReadAsStreamAsync();
        var jsonNode = await JsonNode.ParseAsync(stream);

        var asObj = jsonNode as JsonObject;

        return asObj!;
    }

    /// <summary>
    /// Convert generic jsonnodes into our Item DTOs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="allItems"></param>
    /// <returns></returns>
    public ResultObject<List<T>> MapReleases<T>(JsonArray allItems) where T : Item, new()
    {
        var result = new ResultObject<List<T>>();

        // We'll use a concurrent dictionary
        // As that way we can avoid any duplicates by using the unique id as a key
        ConcurrentDictionary<int, T> bag = new();

        Parallel.ForEach(allItems, itemNode =>
        {
            int id = itemNode.GetPropertyValue<int>("id");

            var release = new T()
            {
                Id = id,
                // TODO artists is an array so we need to handle this
                // Double check data for an example of this
                ArtistName = itemNode.GetPropertyValue<string>("basic_information", "artists", "0", "name"),
                ReleaseName = itemNode.GetPropertyValue<string>("basic_information", "title"),
                ReleaseYear = itemNode.GetPropertyValue<int>("basic_information", "year"),
                DateAdded = itemNode.GetPropertyValue<DateTime>("date_added"),
                Thumbnail = itemNode.GetPropertyValue<Uri>("basic_information", "thumb"),
                CoverImage = itemNode.GetPropertyValue<Uri>("basic_information", "cover_image"),
                Genres = itemNode.GetPropertyValue<List<string>, string>("basic_information", "genres"),
                Styles = itemNode.GetPropertyValue<List<string>, string>("basic_information", "styles"),
                FormatType = GetFormat(itemNode.GetPropertyValue<JsonArray>("basic_information", "formats")!)
            };

            bag.TryAdd(id, release);
        });

        result.Result = [.. bag.Values];

        return result;
    }

    /// <summary>
    /// Read the format object from a Discogs item
    /// </summary>
    /// <param name="formats"></param>
    /// <returns></returns>
    private static string GetFormat(JsonArray formats)
    {
        if (formats.Count == 0)
        {
            return "";
        }

        foreach (var node in formats)
        {
            var type = node.GetPropertyValue<string>("name");

            // This seeems to appear on some items, potentially an internal tag?
            // Regardless, we want something like "Vinyl" or "CD"
            if (type != "All Media")
            {
                return type!;
            }
        }

        return "";
    }

    /// <summary>
    /// Get items from a Discogs response
    /// These may exist under "releases" or "wants"
    /// Depending on API called
    /// </summary>
    /// <param name="allReleases"></param>
    /// <param name="obj"></param>
    /// <param name="propertyName"></param>
    public void GetEntriesFromPage(JsonArray allReleases, JsonObject obj, string propertyName)
    {
        if (obj!.TryGetPropertyValue(propertyName, out JsonNode? releases))
        {
            var asArray = releases as JsonArray;

            foreach (var item in asArray!)
            {
                // TODO: I don't love having to use DeepClone here
                // Potential issues if this scales up
                allReleases.Add(item!.DeepClone());
            }
        }
    }

    /// <summary>
    /// Handle the pagination object within a Discogs response
    /// And determine if we need to call the API again
    /// </summary>
    /// <param name="apiResponse"></param>
    /// <param name="client"></param>
    /// <param name="username"></param>
    /// <param name="page"></param>
    /// <param name="perPage"></param>
    /// <param name="getPageFunc"></param>
    /// <param name="allReleases"></param>
    /// <param name="itemPropertyName"></param>
    /// <returns></returns>
    public async Task CheckAllPages(
    JsonObject apiResponse,
    HttpClient client,
    string username,
    int page,
    int perPage,
    Func<HttpClient, string, int, int, Task<ResultObject<HttpResponseMessage>>> getPageFunc,
    JsonArray allReleases,
    string itemPropertyName)
    {
        if (apiResponse!.TryGetPropertyValue("pagination", out JsonNode? pagination))
        {
            var pageInfo = new Pagination(pagination);

            // if there are other pages, let's get to work
            if (pageInfo.Page < pageInfo.Pages)
            {
                var callsToMake = pageInfo.Pages - pageInfo.Page;

                for (int i = 0; i < callsToMake; i++)
                {
                    var nextPage = await getPageFunc(client, username, ++page, perPage);

                    if (nextPage.HasError)
                    {
                        // Handle or return error
                        return;
                    }

                    var nextObj = await GetJsonObjectFromResponseAsync(nextPage.Result!);
                    GetEntriesFromPage(allReleases, nextObj!, itemPropertyName);
                }
            }
        }
    }
}

public interface IDiscogsApiHelper
{
    /// <summary>
    /// Create a HTTP Client to Discogs using our token
    /// </summary>
    /// <returns></returns>
    HttpClient CreateClient();

    /// <summary>
    /// Get the json content from a HttpResponseMessage
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    Task<JsonObject> GetJsonObjectFromResponseAsync(HttpResponseMessage response);

    /// <summary>
    /// Convert generic jsonnodes into our Item DTOs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="allItems"></param>
    /// <returns></returns>
    ResultObject<List<T>> MapReleases<T>(JsonArray allReleases) where T : Item, new();

    /// <summary>
    /// Get items from a Discogs response
    /// These may exist under "releases" or "wants"
    /// Depending on API called
    /// </summary>
    /// <param name="allReleases"></param>
    /// <param name="obj"></param>
    /// <param name="propertyName"></param>
    void GetEntriesFromPage(JsonArray allReleases, JsonObject obj, string propertyName);

    /// <summary>
    /// Handle the pagination object within a Discogs response
    /// And determine if we need to call the API again
    /// </summary>
    /// <param name="apiResponse"></param>
    /// <param name="client"></param>
    /// <param name="username"></param>
    /// <param name="page"></param>
    /// <param name="perPage"></param>
    /// <param name="getPageFunc"></param>
    /// <param name="allReleases"></param>
    /// <param name="itemPropertyName"></param>
    /// <returns></returns>
    Task CheckAllPages(
    JsonObject apiResponse,
    HttpClient client,
    string username,
    int page,
    int perPage,
    Func<HttpClient, string, int, int, Task<ResultObject<HttpResponseMessage>>> getPageFunc,
    JsonArray allReleases,
    string itemPropertyName);
}