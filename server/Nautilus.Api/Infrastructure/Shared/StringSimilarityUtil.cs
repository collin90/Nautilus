namespace Nautilus.Api.Infrastructure.Shared;

/// <summary>
/// Utility class for string similarity and distance calculations.
/// </summary>
public static class StringSimilarityUtil
{
    /// <summary>
    /// Calculates similarity between two strings using Levenshtein distance.
    /// Returns a value between 0 (completely different) and 1 (identical).
    /// </summary>
    public static double CalculateSimilarity(string source, string target)
    {
        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0;

        if (source == target)
            return 1;

        int distance = LevenshteinDistance(source, target);
        int maxLength = Math.Max(source.Length, target.Length);
        return 1.0 - ((double)distance / maxLength);
    }

    /// <summary>
    /// Calculates the Levenshtein distance between two strings.
    /// The Levenshtein distance is the minimum number of single-character edits
    /// (insertions, deletions, or substitutions) required to change one string into the other.
    /// </summary>
    public static int LevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source))
            return target?.Length ?? 0;
        if (string.IsNullOrEmpty(target))
            return source.Length;

        int[,] distance = new int[source.Length + 1, target.Length + 1];

        for (int i = 0; i <= source.Length; i++)
            distance[i, 0] = i;
        for (int j = 0; j <= target.Length; j++)
            distance[0, j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                distance[i, j] = Math.Min(
                    Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1),
                    distance[i - 1, j - 1] + cost);
            }
        }

        return distance[source.Length, target.Length];
    }
}
