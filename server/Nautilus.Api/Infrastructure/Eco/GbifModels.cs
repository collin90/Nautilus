using Nautilus.Api.Infrastructure.Shared;

namespace Nautilus.Api.Infrastructure.Eco;

/// <summary>
/// DTOs for GBIF (Global Biodiversity Information Facility) API responses.
/// </summary>
/// 
/// <summary>
/// Root response object from GBIF species search API.
/// </summary>
internal class GbifResult
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public bool EndOfRecords { get; set; }
    public List<GbifSpecies> Results { get; set; } = [];
}

/// <summary>
/// Represents a species entry from GBIF API with taxonomic hierarchy.
/// </summary>
internal class GbifSpecies
{
    public int Key { get; set; }
    public string? ScientificName { get; set; }
    public string? CanonicalName { get; set; }
    public string? Kingdom { get; set; }
    public string? Phylum { get; set; }
    public string? Class { get; set; }
    public string? Order { get; set; }
    public string? Family { get; set; }
    public string? Genus { get; set; }
    public string? Species { get; set; }
    // GBIF returns vernacularNames as objects with vernacularName and language properties
    public List<GbifVernacularName>? VernacularNames { get; set; }

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

/// <summary>
/// Root response object from GBIF vernacular names API.
/// </summary>
internal class GbifVernacularResult
{
    public List<GbifVernacularName> Results { get; set; } = [];
}

/// <summary>
/// Represents a vernacular (common) name from GBIF with language information.
/// </summary>
internal class GbifVernacularName
{
    public string? VernacularName { get; set; }
    public string? Language { get; set; }
}
