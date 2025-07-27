using DiscogsProxy.DTO;
using Microsoft.EntityFrameworkCore;
using DiscogsProxy.Constants;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DiscogsProxy.Workers;

/// <summary>
/// Class to handle generating random facts
/// </summary>
public partial class FactGenerator(DiscogsContext discogsContext) : IFactGenerator
{
    private readonly DiscogsContext _context = discogsContext;
    private Random _rand = new();

    /// <summary>
    /// Generate a psuedo random fact about the dataset
    /// </summary>
    /// <returns></returns>
    public string GenerateFact()
    {
        var tableNum = _rand.Next(10);

        if (tableNum <= 7)
        {
            // Collection fact
            return GenerateCollectionFact()!;
        }
        else if (tableNum <= 9)
        {
            // Genres fact
            return GenerateGenresFact();
        }
        else
        {
            // Styles fact
            return GenerateStylesFact();
        }
    }

    /// <summary>
    /// Generate a fact about the collection table
    /// </summary>
    /// <returns></returns>
    public string GenerateCollectionFact()
    {
        var collectionNum = _rand.Next(10);

        if (collectionNum <= 7)
        {
            // Artist fact
            var artistReleaseCounts = _context.Collection
                .AsNoTracking()
                .ToList() // Pull data into memory if ArtistName is List<string>
                .SelectMany(x => x.ArtistName!) // Flatten the list of artist names
                .GroupBy(name => name)
                .OrderByDescending(g => g.Count()) // Order by number of appearances
                .ThenBy(g => g.Key)
                .ToList(); // Keep it as list to preserve order & allow indexing

            int index = _rand.Next(artistReleaseCounts.Count - 1);
            var entry = artistReleaseCounts[index];

            if (entry.Count() == 1)
            {
                var release = _context.Collection.First(x => x.ArtistName!.Contains(entry.Key));
                return string.Format(FactTemplates.SingleItemFor, release.ReleaseName, entry.Key);
            }
            else
            {
                return string.Format(FactTemplates.PopularArtist, entry.Key, GetHasOrHave(entry.Key), entry.Count(), MapIntToPlace(index));
            }
        }
        else
        {
            var groupedByMonth = _context.Collection
                .Select(x => x.DateAdded)
                .GroupBy(d => new { d.Year, d.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToList();

            var rand = new Random();
            var randomIndex = rand.Next(groupedByMonth.Count - 1);
            var randomMonth = groupedByMonth[randomIndex];

            return string.Format(FactTemplates.Added, randomMonth.Count, CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(randomMonth.Month), randomMonth.Year);
        }
    }

    /// <summary>
    /// Generate a fact about the genres table
    /// </summary>
    /// <returns></returns>
    public string GenerateGenresFact()
    {
        return GenerateInfoFact(_context.Genres, FactTemplates.PopularGenre);
    }

    /// <summary>
    /// Generate a fact about the styles table
    /// </summary>
    /// <returns></returns>
    public string GenerateStylesFact()
    {
        return GenerateInfoFact(_context.Styles, FactTemplates.PopularStyle);
    }

    /// <summary>
    /// Common method for both Styles and Genres to help generate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dbSet"></param>
    /// <param name="stringTemplate"></param>
    /// <returns></returns>
    private string GenerateInfoFact<T>(DbSet<T> dbSet, string stringTemplate) where T : MusicInfo, new()
    {
        // Artist fact
        var infoCount = dbSet
            .AsNoTracking()
            .ToList()
            .OrderByDescending(g => g.Instances)
            .ThenBy(g => g.Text)
            .ToList();

        int index = _rand.Next(infoCount.Count - 1);
        var entry = infoCount[index];

        return string.Format(stringTemplate, entry.Instances, entry.Text, MapIntToPlace(index));
    }

    /// <summary>
    /// Determine if we should use "Has" or "Have"
    /// </summary>
    /// <param name="subject"></param>
    /// <returns></returns>
    public string GetHasOrHave(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject)) return "has"; // Default fallback

        // Remove trailing number in brackets, e.g., "Records (12)" → "Records"
        var cleaned = IntAtEnd().Replace(subject.Trim(), "");

        // Basic plural detection
        bool isPlural = cleaned.EndsWith("s", StringComparison.OrdinalIgnoreCase)
                        && !cleaned.EndsWith("ss", StringComparison.OrdinalIgnoreCase); // crude singular exception

        return isPlural ? "have" : "has";
    }

    /// <summary>
    /// Convert an int to it's ordinal version
    /// 1 = 1st, 2 = 2nd etc
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public static string MapIntToPlace(int index)
    {
        // index will be 0 indexed. We need it one more
        var incremented = index++;
        int lastTwoDigits = incremented % 100;
        int lastDigit = incremented % 10;

        string suffix = (lastTwoDigits >= 11 && lastTwoDigits <= 13) ? "th"
                     : (lastDigit == 1) ? "st"
                     : (lastDigit == 2) ? "nd"
                     : (lastDigit == 3) ? "rd"
                     : "th";

        return $"{incremented}{suffix}";
    }

    [GeneratedRegex(@"\s*\(\d+\)$")]
    private static partial Regex IntAtEnd();
}

/// <summary>
/// Interface for FactGenerator
/// </summary>
public interface IFactGenerator
{
    /// <summary>
    /// Generate a psuedo random fact about the dataset
    /// </summary>
    /// <returns></returns>
    string GenerateFact();
}