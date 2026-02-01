using Nautilus.Api.Data;
using Nautilus.Api.Backend;
using Nautilus.Api.Clients;
using Nautilus.Api.Lib;

namespace Nautilus.Api.Services.Eco;

/// <summary>
/// Species search service that coordinates between GBIF client and backend storage
/// </summary>
public class SpeciesSearchService(
    ILogger<SpeciesSearchService> logger,
    IGbifClient gbifClient,
    ISpeciesBackend backend) : ISpeciesSearchService
{
    private readonly ILogger<SpeciesSearchService> _logger = logger;
    private readonly IGbifClient _gbifClient = gbifClient;
    private readonly ISpeciesBackend _backend = backend;

    // Relevance filtering
    private const double _minRelevanceScore = 100.0;

    public async Task<List<SpeciesSearchResult>> SearchSpeciesAsync(string query, string? kingdom = null)
    {
        try
        {
            // 1. Check backend cache first
            _logger.LogDebug("Searching species for query: {Query}, kingdom: {Kingdom}", query, kingdom ?? "all");
            var cachedResults = await _backend.SearchSpeciesAsync(query, kingdom);

            if (cachedResults.Count != 0)
            {
                _logger.LogDebug("Backend cache hit for query: {Query} with {Count} results", query, cachedResults.Count);
                return cachedResults.Where(r => CalculateRelevanceScore(r, query) >= _minRelevanceScore).ToList();
            }

            _logger.LogDebug("Backend cache miss for query: {Query}", query);

            // 2. Cache miss - call GBIF API via client
            GbifResult gbifResult = await _gbifClient.SearchSpeciesAsync(query);

            // 3. Filter by kingdom if specified
            if (!string.IsNullOrWhiteSpace(kingdom) && !kingdom.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                gbifResult.Results = [.. gbifResult.Results.Where(s => s.Kingdom != null && s.Kingdom.Equals(kingdom, StringComparison.OrdinalIgnoreCase))];
                _logger.LogDebug("Filtered to {Count} results for kingdom: {Kingdom}", gbifResult.Results.Count, kingdom);
            }

            // 4. Preprocess and deduplicate GBIF data
            var processedSpecies = GbifUtils.PreprocessGbifSpecies(gbifResult.Results);
            _logger.LogDebug("Processing {Count} unique species for query: {Query}", processedSpecies.Count, query);

            // 5. Build species results and populate backend cache
            var speciesResults = await BuildSpeciesResultsAndCache(processedSpecies, query);

            // 6. Filter and sort by relevance
            var filteredResults = FilterAndSortByRelevance(speciesResults, query);

            _logger.LogDebug("Search completed for query: {Query}, found {Count} unique species", query, filteredResults.Count);
            return filteredResults;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching species for query: {Query}", query);
            return [];
        }
    }

    public Task<string?> GetSpeciesImageAsync(string scientificName)
    {
        _logger.LogDebug("SpeciesSearchService.GetSpeciesImageAsync called for: {ScientificName}", scientificName);
        return _backend.GetSpeciesImageAsync(scientificName);
    }

    private double CalculateRelevanceScore(SpeciesSearchResult result, string query)
    {
        var tempTree = new TaxonomicTree
        {
            Species = result.ScientificName,
            VernacularNames = result.VernacularNames
        };
        return SpeciesRelevanceUtil.CalculateRelevanceScore(tempTree, query.Trim().ToLowerInvariant());
    }

    private List<SpeciesSearchResult> FilterAndSortByRelevance(List<SpeciesSearchResult> results, string query)
    {
        var normalizedQuery = query.Trim().ToLowerInvariant();

        var scoresDict = results.ToDictionary(
            r => r,
            r => CalculateRelevanceScore(r, normalizedQuery)
        );

        return [.. results
            .Where(r => scoresDict.TryGetValue(r, out var score) && score >= _minRelevanceScore)
            .OrderByDescending(r => scoresDict.TryGetValue(r, out var score) ? score : 0)
            .ThenBy(r => r.ScientificName)];
    }

    private async Task<List<SpeciesSearchResult>> BuildSpeciesResultsAndCache(List<GbifSpecies> gbifSpeciesList, string query)
    {
        var results = new List<SpeciesSearchResult>();
        var normalizedQuery = query.Trim().ToLowerInvariant();

        foreach (var gbifSpecies in gbifSpeciesList)
        {
            var scientificName = gbifSpecies.ScientificName ?? "Unknown";
            _logger.LogDebug("Processing species: Key={Key}, ScientificName={ScientificName}", gbifSpecies.Key, scientificName);

            // Fetch vernacular names from GBIF via client
            var vernacularNames = await _gbifClient.GetVernacularNamesAsync(gbifSpecies.Key);

            // Store species in backend
            var species = new Species
            {
                ScientificName = scientificName,
                Kingdom = gbifSpecies.Kingdom,
                UsageKey = gbifSpecies.Key
            };
            var speciesId = await _backend.StoreSpeciesAsync(species);

            // Store common names in backend
            foreach (var vernacularName in vernacularNames)
            {
                await _backend.StoreCommonNameAsync(speciesId, vernacularName, "GBIF");
            }

            // Fetch image from backend (which handles iNaturalist call and caching)
            var imageUrl = await _backend.GetSpeciesImageAsync(scientificName);

            // Create result object
            var result = new SpeciesSearchResult
            {
                ScientificName = scientificName,
                Kingdom = gbifSpecies.Kingdom,
                VernacularNames = vernacularNames,
                ImageUrl = imageUrl
            };
            results.Add(result);

            // Calculate and store relevance score in backend cache
            var relevanceScore = CalculateRelevanceScore(result, normalizedQuery);
            await _backend.StoreSearchCacheAsync(query, speciesId, relevanceScore);
        }

        return results;
    }
}
