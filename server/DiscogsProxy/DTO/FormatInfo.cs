namespace DiscogsProxy.DTO;

/// <summary>
/// Hold info about the type of release
/// </summary>
public class FormatInfo
{
    /// <summary>
    /// FormatType - Vinyl, CD, Cassette
    /// </summary>
    public string? FormatType { get; set; }

    /// <summary>
    /// Information about the physical disc
    /// </summary>
    public List<DiscInfo>? DiscInfo { get; set; }
}

/// <summary>
/// Info about the individual discs in the release
/// </summary>
public class DiscInfo
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Associated text, colour, size etc
    /// </summary>
    public string? Text { get; set; }
}