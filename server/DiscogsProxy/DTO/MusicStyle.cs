
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
    public int Id { get; set; }

    public string? Text { get; set; }
}