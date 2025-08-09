namespace DiscogsProxy.DTO;

public class CollectionItem : Item;

public class WantlistItem : Item;

/// <summary>
/// Information for an item in a user's collection
/// </summary>
public class Item
{
    /// <summary>
    /// ReleaseKey
    /// This is the primary key for the collection item
    /// </summary>
    public int ReleaseKey { get; set; }

    /// <summary>
    /// Id - From Discogs
    /// This is the Discogs Release ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ArtistNames
    /// </summary>
    public List<string>? ArtistName { get; set; }

    /// <summary>
    /// ReleaseName
    /// </summary>
    public string? ReleaseName { get; set; }

    /// <summary>
    /// ReleaseYear
    /// </summary>
    public int ReleaseYear { get; set; }

    /// <summary>
    /// DateAdded
    /// </summary>
    public DateTime DateAdded { get; set; }

    /// <summary>
    /// Thumbnail
    /// </summary>
    public Uri? Thumbnail { get; set; }

    /// <summary>
    /// Cover Image - Higher res Thumb
    /// </summary>
    public Uri? CoverImage { get; set; }

    /// <summary>
    /// Genres
    /// </summary>
    public List<string>? Genres { get; set; }

    /// <summary>
    /// Styles
    /// </summary>
    public List<string>? Styles { get; set; }

    /// <summary>
    /// Format info
    /// </summary>
    public FormatInfo? FormatInfo { get; set; }
}