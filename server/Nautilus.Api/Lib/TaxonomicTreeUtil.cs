using Nautilus.Api.Data;

namespace Nautilus.Api.Lib;

/// <summary>
/// Utility class for taxonomic tree operations and filtering.
/// Note: For building taxonomic trees from species data, use ITaxonomicDataService.
/// </summary>
public static class TaxonomicTreeUtil
{
    /// <summary>
    /// Applies kingdom filtering and sorting using pre-calculated relevance scores.
    /// </summary>
    public static List<TaxonomicTree> ApplyKingdomFilterAndSort(
        List<TaxonomicTree> trees,
        Dictionary<TaxonomicTree, double> relevanceScores,
        double minRelevanceScore,
        string? kingdom)
    {
        // Normalize kingdom filter
        var kingdomFilter = kingdom?.Trim().ToLowerInvariant();

        // Filter by kingdom if specified
        var filtered = trees;
        if (!string.IsNullOrWhiteSpace(kingdomFilter) && kingdomFilter != "all")
        {
            filtered = [.. trees.Where(t => t.Kingdom != null && t.Kingdom.Equals(kingdom, StringComparison.OrdinalIgnoreCase))];
        }

        // Filter by minimum relevance score and sort
        return [.. filtered
            .Where(t => relevanceScores.TryGetValue(t, out var score) && score >= minRelevanceScore)
            .OrderByDescending(t => relevanceScores.TryGetValue(t, out var score) ? score : 0)
            .ThenBy(t => GetKingdomSortOrder(t.Kingdom))
            .ThenBy(t => t.Species)];
    }

    /// <summary>
    /// Returns sort order for kingdoms (lower number = higher priority).
    /// </summary>
    public static int GetKingdomSortOrder(string? kingdom)
    {
        return kingdom?.ToLowerInvariant() switch
        {
            "animalia" => 1,
            "plantae" => 2,
            "fungi" => 3,
            "bacteria" => 4,
            "archaea" => 5,
            "protista" => 6,
            "chromista" => 7,
            _ => 99 // Unknown kingdoms go last
        };
    }
}
