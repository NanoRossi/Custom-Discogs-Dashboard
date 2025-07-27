
namespace DiscogsProxy.DTO;

/// <summary>
/// Styles
/// </summary>
public class MusicStyle : MusicInfo;

/// <summary>
/// Genres
/// </summary>
public class MusicGenre : MusicInfo;

/// <summary>
/// Base class for Styles and Genres
/// </summary>
public class MusicInfo
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The name of the style/genre
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// How many times this style/genre appears
    /// </summary>
    public int Instances { get; set; }
}