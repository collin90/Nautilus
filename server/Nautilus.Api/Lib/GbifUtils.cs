using Nautilus.Api.Data;

namespace Nautilus.Api.Lib;

public static class GbifUtils
{
    /// <summary>
    /// Normalizes scientific names to "genus species" format and merges duplicate records.
    /// </summary>
    public static List<GbifSpecies> PreprocessGbifSpecies(List<GbifSpecies> species)
    {
        var normalizedSpecies = new Dictionary<string, GbifSpecies>(StringComparer.OrdinalIgnoreCase);

        foreach (var gbifSpecies in species)
        {
            // Normalize scientific name to "genus species" format
            var scientificName = gbifSpecies.ScientificName ?? gbifSpecies.CanonicalName ?? string.Empty;
            var normalizedName = EcoUtils.NormalizeScientificName(scientificName);

            // Skip if name cannot be normalized to "genus species" format
            if (normalizedName == null)
                continue;

            if (normalizedSpecies.TryGetValue(normalizedName, out var existing))
            {
                // Merge records - fill in missing data from new record
                existing.Kingdom ??= gbifSpecies.Kingdom;
                existing.Phylum ??= gbifSpecies.Phylum;
                existing.Class ??= gbifSpecies.Class;
                existing.Order ??= gbifSpecies.Order;
                existing.Family ??= gbifSpecies.Family;
                existing.Genus ??= gbifSpecies.Genus;
                existing.Species ??= gbifSpecies.Species;

                // Keep the first Key encountered
                if (existing.Key == 0 && gbifSpecies.Key != 0)
                    existing.Key = gbifSpecies.Key;
            }
            else
            {
                // Create a copy with normalized name
                var normalized = new GbifSpecies
                {
                    Key = gbifSpecies.Key,
                    ScientificName = normalizedName,
                    CanonicalName = gbifSpecies.CanonicalName,
                    Kingdom = gbifSpecies.Kingdom,
                    Phylum = gbifSpecies.Phylum,
                    Class = gbifSpecies.Class,
                    Order = gbifSpecies.Order,
                    Family = gbifSpecies.Family,
                    Genus = gbifSpecies.Genus,
                    Species = gbifSpecies.Species,
                    VernacularNames = gbifSpecies.VernacularNames
                };
                normalizedSpecies[normalizedName] = normalized;
            }
        }

        return [.. normalizedSpecies.Values];
    }
}