namespace DiscogsProxy.Constants;

/// <summary>
/// Constants to represent Database status
/// </summary>
public static class DbStatus
{
    /// <summary>
    /// Db is up and contains Data
    /// </summary>
    public const string Active = "Active";

    /// <summary>
    /// Db can be connected to but is currently empty
    /// </summary>
    public const string Empty = "Empty";

    /// <summary>
    /// Db cannot be connected to
    /// </summary>
    public const string Disconnected = "Disconnected";
}