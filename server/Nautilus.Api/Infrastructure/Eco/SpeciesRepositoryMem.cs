using System.Text.Json;
using Nautilus.Api.Application.Eco.Species;

namespace Nautilus.Api.Infrastructure.Eco;

public class SpeciesRepositoryMem(ILogger<SpeciesRepositoryMem> logger) : ISpeciesRepository
{
    private static readonly HttpClient _httpClient = new();

    // GBIF API constants
    private const string _gbifBaseUrlV1 = "https://api.gbif.org/v1";
    private const string _gbifSpeciesSearchEndpoint = "/species/search";
    private const string _gbifVernacularNamesEndpointTemplate = "/species/{0}/vernacularNames";
    private const int _gbifSearchLimit = 100;
    private const string _gbifSpeciesRank = "SPECIES";
    private const string _gbifAcceptedStatus = "ACCEPTED";

    // Relevance filtering
    private const double _minRelevanceScore = 50.0; // Minimum score to include in results

    // JSON deserialization options
    private readonly JsonSerializerOptions _jsonSerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private async Task<GbifResult> GetGbifSpeciesResult(string query)
    {
        var gbifUrl = $"{_gbifBaseUrlV1}{_gbifSpeciesSearchEndpoint}?q={Uri.EscapeDataString(query)}&rank={_gbifSpeciesRank}&status={_gbifAcceptedStatus}&limit={_gbifSearchLimit}";
        var response = await _httpClient.GetAsync(gbifUrl);
        logger.LogDebug("Called GBIF API for query: {Query}", query);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogDebug("GBIF API call failed with status code: {StatusCode}", response.StatusCode);
            return new GbifResult();
        }

        var jsonContent = await response.Content.ReadAsStringAsync();

        var gbifResult = JsonSerializer.Deserialize<GbifResult>(jsonContent, _jsonSerOptions);
        if (gbifResult == null || gbifResult.Results == null)
        {
            logger.LogDebug("GBIF API returned null or invalid data for query: {Query}", query);
            return new GbifResult();
        }
        logger.LogDebug("GBIF API returned {Count} results for query: {Query}", gbifResult.Results.Count, query);
        logger.LogDebug("{JsonContent}", jsonContent);

        return gbifResult;
    }

    public async Task<List<TaxonomicTree>> SearchSpeciesAsync(string query, string? kingdom = null)
    {
        try
        {
            // 1. Check cache first
            logger.LogDebug("Searching species for query: {Query}, kingdom: {Kingdom}", query, kingdom ?? "all");
            if (EcoStoreMem.SearchCache.TryGetValue(query, out var cachedResults))
            {
                var cachedTreesWithScores = cachedResults
                    .Select(cache => new
                    {
                        Species = EcoStoreMem.Species.TryGetValue(cache.SpeciesId, out var species) ? species : null,
                        cache.RelevanceScore
                    })
                    .Where(x => x.Species != null)
                    .Select(x => new
                    {
                        Tree = BuildTaxonomicTree(x.Species!),
                        Score = x.RelevanceScore
                    })
                    .ToList();

                if (cachedTreesWithScores.Count != 0)
                {
                    logger.LogDebug("Cache hit for query: {Query} with {Count} results", query, cachedTreesWithScores.Count);

                    // Apply kingdom filter and sort using cached scores
                    var filteredCached = ApplyKingdomFilterAndSort(cachedTreesWithScores.Select(x => x.Tree).ToList(),
                        cachedTreesWithScores.ToDictionary(x => x.Tree, x => x.Score), kingdom);

                    return filteredCached;
                }
            }
            logger.LogDebug("Cache miss for query: {Query}", query);

            // 2. Cache miss - call GBIF API with increased limit
            GbifResult gbifResult = await GetGbifSpeciesResult(query);

            // 3. Preprocess and deduplicate GBIF data
            var processedSpecies = PreprocessGbifSpecies(gbifResult.Results);
            logger.LogDebug("Processing {Count} unique species for query: {Query}", processedSpecies.Count, query);

            // 4. Build taxonomic trees and populate cache
            var taxonomicTrees = await BuildTaxonomicTreesAndCache(processedSpecies, query);

            // 5. Filter and sort by relevance and kingdom
            var filteredTrees = FilterAndSortByRelevance(taxonomicTrees, query, kingdom);

            logger.LogDebug("Search completed for query: {Query}, found {Count} unique species", query, filteredTrees.Count);
            return filteredTrees;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching species for query: {Query}", query);
            return [];
        }
    }

    /// <summary>
    /// Filters results by kingdom and sorts them by relevance to the search query.
    /// Relevance is based on: 1) Scientific name match, 2) Vernacular name match, 3) Similarity.
    /// </summary>
    private static List<TaxonomicTree> FilterAndSortByRelevance(List<TaxonomicTree> trees, string query, string? kingdom)
    {
        var normalizedQuery = query.Trim().ToLowerInvariant();

        // Calculate scores for all trees
        var scoresDict = trees.ToDictionary(
            t => t,
            t => SpeciesRelevanceUtil.CalculateRelevanceScore(t, normalizedQuery)
        );

        return ApplyKingdomFilterAndSort(trees, scoresDict, kingdom);
    }

    /// <summary>
    /// Applies kingdom filtering and sorting using pre-calculated relevance scores.
    /// </summary>
    private static List<TaxonomicTree> ApplyKingdomFilterAndSort(
        List<TaxonomicTree> trees,
        Dictionary<TaxonomicTree, double> relevanceScores,
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
            .Where(t => relevanceScores.TryGetValue(t, out var score) && score >= _minRelevanceScore)
            .OrderByDescending(t => relevanceScores.TryGetValue(t, out var score) ? score : 0)
            .ThenBy(t => GetKingdomSortOrder(t.Kingdom))
            .ThenBy(t => t.Species)];
    }

    /// <summary>
    /// Returns sort order for kingdoms (lower number = higher priority).
    /// </summary>
    private static int GetKingdomSortOrder(string? kingdom)
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

    /// <summary>
    /// Normalizes scientific names to "genus species" format and merges duplicate records.
    /// </summary>
    private static List<GbifSpecies> PreprocessGbifSpecies(List<GbifSpecies> species)
    {
        var normalizedSpecies = new Dictionary<string, GbifSpecies>(StringComparer.OrdinalIgnoreCase);

        foreach (var gbifSpecies in species)
        {
            // Normalize scientific name to "genus species" format
            var scientificName = gbifSpecies.ScientificName ?? gbifSpecies.CanonicalName ?? string.Empty;
            var normalizedName = NormalizeScientificName(scientificName);

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

    /// <summary>
    /// Normalizes scientific name to "genus species" format by removing author/discoverer info.
    /// Example: "Panthera leo (Linnaeus, 1758)" -> "Panthera leo"
    /// Returns null if the name cannot be normalized to genus species format.
    /// </summary>
    private static string? NormalizeScientificName(string scientificName)
    {
        if (string.IsNullOrWhiteSpace(scientificName))
            return null;

        // Remove anything in parentheses (discoverer/author info)
        var parenIndex = scientificName.IndexOf('(');
        if (parenIndex > 0)
            scientificName = scientificName[..parenIndex].Trim();

        // Split into words and take first two (genus species)
        var parts = scientificName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
            return $"{parts[0]} {parts[1]}";

        // Cannot normalize to "genus species" format
        return null;
    }

    /// <summary>
    /// Builds taxonomic trees from preprocessed GBIF species and populates cache.
    /// </summary>
    private async Task<List<TaxonomicTree>> BuildTaxonomicTreesAndCache(List<GbifSpecies> gbifSpeciesList, string query)
    {
        var taxonomicTrees = new List<TaxonomicTree>();
        var normalizedQuery = query.Trim().ToLowerInvariant();

        foreach (var gbifSpecies in gbifSpeciesList)
        {
            var scientificName = gbifSpecies.ScientificName ?? "Unknown";
            logger.LogDebug("Processing species: Key={Key}, ScientificName={ScientificName}", gbifSpecies.Key, scientificName);

            // Fetch vernacular names from GBIF species detail endpoint
            var vernacularNames = await FetchVernacularNamesAsync(gbifSpecies.Key);

            // Store taxonomic hierarchy
            var kingdomId = GetOrCreateKingdom(gbifSpecies.Kingdom);
            var phylumId = GetOrCreatePhylum(gbifSpecies.Phylum, kingdomId);
            var classId = GetOrCreateClass(gbifSpecies.Class, phylumId);
            var orderId = GetOrCreateOrder(gbifSpecies.Order, classId);
            var familyId = GetOrCreateFamily(gbifSpecies.Family, orderId);
            var genusId = GetOrCreateGenus(gbifSpecies.Genus, familyId);

            // Store species
            var speciesId = EcoStoreMem.GetNextSpeciesId();
            var species = new Species
            {
                Id = speciesId,
                ScientificName = scientificName,
                GenusId = genusId,
                UsageKey = gbifSpecies.Key
            };
            EcoStoreMem.Species[speciesId] = species;

            // Store common names
            foreach (var vernacularName in vernacularNames)
            {
                var commonNameId = Guid.NewGuid();
                var taxonCommonName = new TaxonCommonName
                {
                    Id = commonNameId,
                    SpeciesId = speciesId,
                    CommonName = vernacularName,
                    Source = "GBIF"
                };
                EcoStoreMem.TaxonCommonNames[commonNameId] = taxonCommonName;
            }

            // Build taxonomic tree
            var tree = new TaxonomicTree
            {
                Kingdom = gbifSpecies.Kingdom,
                Phylum = gbifSpecies.Phylum,
                Class = gbifSpecies.Class,
                Order = gbifSpecies.Order,
                Family = gbifSpecies.Family,
                Genus = gbifSpecies.Genus,
                Species = scientificName,
                VernacularNames = vernacularNames
            };
            taxonomicTrees.Add(tree);

            // Calculate and store relevance score in cache
            var relevanceScore = SpeciesRelevanceUtil.CalculateRelevanceScore(tree, normalizedQuery);

            var searchCacheEntry = new SearchCache
            {
                QueryText = query,
                SpeciesId = speciesId,
                RelevanceScore = relevanceScore,
                CreatedAt = DateTime.UtcNow
            };

            if (!EcoStoreMem.SearchCache.ContainsKey(query))
                EcoStoreMem.SearchCache[query] = [];

            EcoStoreMem.SearchCache[query].Add(searchCacheEntry);
        }

        return taxonomicTrees;
    }

    /// <summary>
    /// Fetches vernacular names for a species from GBIF API.
    /// </summary>
    private static async Task<List<string>> FetchVernacularNamesAsync(int speciesKey)
    {
        try
        {
            var vernacularUrl = $"{_gbifBaseUrlV1}{string.Format(_gbifVernacularNamesEndpointTemplate, speciesKey)}";
            var response = await _httpClient.GetAsync(vernacularUrl);

            if (!response.IsSuccessStatusCode)
                return [];

            var jsonContent = await response.Content.ReadAsStringAsync();
            var vernacularResult = JsonSerializer.Deserialize<GbifVernacularResult>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (vernacularResult?.Results == null)
                return [];

            return [.. vernacularResult.Results
                .Where(v => !string.IsNullOrWhiteSpace(v.VernacularName))
                .Select(v => v.VernacularName!)
                .Distinct(StringComparer.OrdinalIgnoreCase)];
        }
        catch
        {
            return [];
        }
    }

    private static TaxonomicTree BuildTaxonomicTree(Species species)
    {
        // Traverse hierarchy to build full taxonomic tree
        var genus = EcoStoreMem.Genuses.TryGetValue(species.GenusId, out var g) ? g : null;
        var family = genus != null && EcoStoreMem.Families.TryGetValue(genus.FamilyId, out var f) ? f : null;
        var order = family != null && EcoStoreMem.Orders.TryGetValue(family.OrderId, out var o) ? o : null;
        var classEntity = order != null && EcoStoreMem.Classes.TryGetValue(order.ClassId, out var c) ? c : null;
        var phylum = classEntity != null && EcoStoreMem.Phylums.TryGetValue(classEntity.PhylumId, out var p) ? p : null;
        var kingdom = phylum != null && EcoStoreMem.Kingdoms.TryGetValue(phylum.KingdomId, out var k) ? k : null;

        var vernacularNames = EcoStoreMem.TaxonCommonNames.Values
            .Where(tcn => tcn.SpeciesId == species.Id)
            .Select(tcn => tcn.CommonName)
            .ToList();

        return new TaxonomicTree
        {
            Kingdom = kingdom?.Name,
            Phylum = phylum?.Name,
            Class = classEntity?.Name,
            Order = order?.Name,
            Family = family?.Name,
            Genus = genus?.Name,
            Species = species.ScientificName,
            VernacularNames = vernacularNames
        };
    }

    private static int GetOrCreateKingdom(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Kingdoms.Values.FirstOrDefault(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextKingdomId();
        EcoStoreMem.Kingdoms[id] = new Kingdom { Id = id, Name = name };
        return id;
    }

    private static int GetOrCreatePhylum(string? name, int kingdomId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Phylums.Values.FirstOrDefault(p =>
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && p.KingdomId == kingdomId);
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextPhylumId();
        EcoStoreMem.Phylums[id] = new Phylum { Id = id, Name = name, KingdomId = kingdomId };
        return id;
    }

    private static int GetOrCreateClass(string? name, int phylumId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Classes.Values.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.PhylumId == phylumId);
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextClassId();
        EcoStoreMem.Classes[id] = new Class { Id = id, Name = name, PhylumId = phylumId };
        return id;
    }

    private static int GetOrCreateOrder(string? name, int classId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Orders.Values.FirstOrDefault(o =>
            o.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && o.ClassId == classId);
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextOrderId();
        EcoStoreMem.Orders[id] = new Order { Id = id, Name = name, ClassId = classId };
        return id;
    }

    private static int GetOrCreateFamily(string? name, int orderId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Families.Values.FirstOrDefault(f =>
            f.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && f.OrderId == orderId);
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextFamilyId();
        EcoStoreMem.Families[id] = new Family { Id = id, Name = name, OrderId = orderId };
        return id;
    }

    private static int GetOrCreateGenus(string? name, int familyId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Genuses.Values.FirstOrDefault(g =>
            g.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && g.FamilyId == familyId);
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextGenusId();
        EcoStoreMem.Genuses[id] = new Genus { Id = id, Name = name, FamilyId = familyId };
        return id;
    }
}

// DTOs for GBIF API response
internal class GbifResult
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public bool EndOfRecords { get; set; }
    public List<GbifSpecies> Results { get; set; } = [];
}

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
}

internal class GbifVernacularResult
{
    public List<GbifVernacularName> Results { get; set; } = [];
}

internal class GbifVernacularName
{
    public string? VernacularName { get; set; }
    public string? Language { get; set; }
}
