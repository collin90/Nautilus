using Nautilus.Api.Application.Eco.Species;

namespace Nautilus.Api.Infrastructure.Eco;

/// <summary>
/// Utility class for calculating search relevance scores for taxonomic results.
/// </summary>
public static class SpeciesRelevanceUtil
{
    /// <summary>
    /// Calculates relevance score for a taxonomic tree based on the search query.
    /// Higher score = more relevant.
    /// 
    /// Scoring criteria:
    /// 1. Exact matches (scientific/vernacular names)
    /// 2. Prefix matches
    /// 3. Contains matches
    /// 4. Word-level matches
    /// 5. Levenshtein distance similarity
    /// </summary>
    public static double CalculateRelevanceScore(TaxonomicTree tree, string normalizedQuery)
    {
        double score = 0;

        var scientificName = tree.Species?.ToLowerInvariant() ?? "";
        var vernacularNames = tree.VernacularNames.Select(v => v.ToLowerInvariant()).ToList();

        // 1. Exact scientific name match (highest priority)
        if (scientificName == normalizedQuery)
            score += 1000;

        // 2. Exact vernacular name match
        if (vernacularNames.Any(v => v == normalizedQuery))
            score += 900;

        // 3. Scientific name starts with query
        if (scientificName.StartsWith(normalizedQuery))
            score += 500;

        // 4. Vernacular name starts with query
        if (vernacularNames.Any(v => v.StartsWith(normalizedQuery)))
            score += 450;

        // 5. Scientific name contains query
        if (scientificName.Contains(normalizedQuery))
            score += 300;

        // 6. Vernacular name contains query
        if (vernacularNames.Any(v => v.Contains(normalizedQuery)))
            score += 250;

        // 7. Word-level matches in scientific name
        var scientificWords = scientificName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var queryWords = normalizedQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var queryWord in queryWords)
        {
            if (scientificWords.Any(w => w == queryWord))
                score += 200;
            else if (scientificWords.Any(w => w.StartsWith(queryWord)))
                score += 100;
            else if (scientificWords.Any(w => w.Contains(queryWord)))
                score += 50;
        }

        // 8. Word-level matches in vernacular names
        foreach (var vernacular in vernacularNames)
        {
            var vernacularWords = vernacular.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var queryWord in queryWords)
            {
                if (vernacularWords.Any(w => w == queryWord))
                    score += 150;
                else if (vernacularWords.Any(w => w.StartsWith(queryWord)))
                    score += 75;
                else if (vernacularWords.Any(w => w.Contains(queryWord)))
                    score += 35;
            }
        }

        // 9. Levenshtein distance similarity (bonus for close matches)
        var scientificSimilarity = Shared.StringSimilarityUtil.CalculateSimilarity(scientificName, normalizedQuery);
        score += scientificSimilarity * 100;

        foreach (var vernacular in vernacularNames)
        {
            var vernacularSimilarity = Shared.StringSimilarityUtil.CalculateSimilarity(vernacular, normalizedQuery);
            score += vernacularSimilarity * 80;
        }

        return score;
    }
}
