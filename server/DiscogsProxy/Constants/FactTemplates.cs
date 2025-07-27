namespace DiscogsProxy.Constants;

/// <summary>
/// Constants to represent fact formats
/// </summary>
public static class FactTemplates
{
    /// <summary>
    /// Popular artist
    /// </summary>
    public const string PopularArtist = "\"{0}\" {1} {2} items in the collection, ranking them {3} among artists";

    /// <summary>
    /// Popular genre
    /// </summary>
    public const string PopularGenre = "There are {0} items under \"{1}\", making it the {2} most popular genre";

    /// <summary>
    /// Popular style
    /// </summary>
    public const string PopularStyle = "There are {0} items under \"{1}\", making it the {2} most popular style";

    /// <summary>
    /// Added
    /// </summary>
    public const string Added = "{0} items were added in {1} {2}";

    /// <summary>
    /// Single item in category
    /// </summary>
    public const string SingleItemFor = "\"{0}\" is the only entry for \"{1}\", it's lonely!";
}