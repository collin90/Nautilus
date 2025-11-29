using System.Text.Json;
using Nautilus.Api.Application.Eco.Species;

namespace Nautilus.Api.Infrastructure.Eco;

public class SpeciesRepositoryMem(ILogger<SpeciesRepositoryMem> logger) : ISpeciesRepository
{
    private static readonly HttpClient _httpClient = new();

    public async Task<List<TaxonomicTree>> SearchSpeciesAsync(string query)
    {
        try
        {
            // 1. Check cache first
            logger.LogInformation("Searching species for query: {Query}", query);
            if (EcoStoreMem.SearchCache.TryGetValue(query, out var cachedResults))
            {
                var cachedTrees = cachedResults
                    .Select(cache => EcoStoreMem.Species.TryGetValue(cache.SpeciesId, out var species) ? species : null)
                    .Where(s => s != null)
                    .Select(s => BuildTaxonomicTree(s!))
                    .ToList();

                if (cachedTrees.Count != 0)
                {
                    logger.LogInformation("Cache hit for query: {Query} with {Count} results", query, cachedTrees.Count);
                    return cachedTrees;
                }
            }
            logger.LogInformation("Cache miss for query: {Query}", query);

            // 2. Cache miss - call GBIF API with increased limit
            var gbifUrl = $"https://api.gbif.org/v1/species/search?q={Uri.EscapeDataString(query)}&rank=SPECIES&status=ACCEPTED&limit=100";
            var response = await _httpClient.GetAsync(gbifUrl);
            logger.LogInformation("Called GBIF API for query: {Query}", query);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogInformation("GBIF API call failed with status code: {StatusCode}", response.StatusCode);
                return [];

            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var gbifResult = JsonSerializer.Deserialize<GbifResult>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (gbifResult == null || gbifResult.Results == null)
            {
                logger.LogInformation("GBIF API returned null or invalid data for query: {Query}", query);
                return [];
            }
            logger.LogInformation("GBIF API returned {Count} results for query: {Query}", gbifResult.Results.Count, query);
            logger.LogInformation("{JsonContent}", jsonContent);

            // 3. Sort results with Animalia first, then other kingdoms
            var sortedSpecies = gbifResult.Results
                .OrderByDescending(s => s.Kingdom != null && s.Kingdom.Equals("Animalia", StringComparison.OrdinalIgnoreCase))
                .ThenBy(s => s.Kingdom)
                .ToList();

            logger.LogInformation("Processing {Count} species for query: {Query}", sortedSpecies.Count, query);

            // 4. Build unique taxonomic trees and populate cache
            var taxonomicTrees = new List<TaxonomicTree>();
            var seenSpecies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var gbifSpecies in sortedSpecies)
            {
                var scientificName = gbifSpecies.ScientificName ?? gbifSpecies.CanonicalName ?? "Unknown";

                // Skip duplicates based on scientific name
                if (seenSpecies.Contains(scientificName))
                    continue;

                seenSpecies.Add(scientificName);
                logger.LogInformation("Processing unique species: Key={Key}, ScientificName={ScientificName}", gbifSpecies.Key, scientificName);

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

                // Store in search cache
                var searchCacheEntry = new SearchCache
                {
                    QueryText = query,
                    SpeciesId = speciesId,
                    Confidence = 1.0m,
                    CreatedAt = DateTime.UtcNow
                };

                if (!EcoStoreMem.SearchCache.ContainsKey(query))
                    EcoStoreMem.SearchCache[query] = [];

                EcoStoreMem.SearchCache[query].Add(searchCacheEntry);
            }

            logger.LogInformation("Search completed for query: {Query}, found {Count} unique species", query, taxonomicTrees.Count);
            return taxonomicTrees;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching species for query: {Query}", query);
            return [];
        }
    }

    private async Task<List<string>> FetchVernacularNamesAsync(int speciesKey)
    {
        try
        {
            var vernacularUrl = $"https://api.gbif.org/v1/species/{speciesKey}/vernacularNames";
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

            return vernacularResult.Results
                .Where(v => !string.IsNullOrWhiteSpace(v.VernacularName))
                .Select(v => v.VernacularName!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch
        {
            return [];
        }
    }

    private TaxonomicTree BuildTaxonomicTree(Species species)
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
    public List<GbifSpecies> Results { get; set; } = new();
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
    public List<GbifVernacularName> Results { get; set; } = new();
}

internal class GbifVernacularName
{
    public string? VernacularName { get; set; }
    public string? Language { get; set; }
}
