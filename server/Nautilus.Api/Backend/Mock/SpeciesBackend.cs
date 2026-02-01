using Nautilus.Api.Data;
using Nautilus.Api.Clients;

namespace Nautilus.Api.Backend.Mock;

/// <summary>
/// In-memory implementation of species backend with caching for local testing
/// </summary>
public class SpeciesBackend(
    ILogger<SpeciesBackend> logger,
    IINaturalistClient iNaturalistClient) : ISpeciesBackend
{
    private readonly ILogger<SpeciesBackend> _logger = logger;
    private readonly IINaturalistClient _iNaturalistClient = iNaturalistClient;

    public async Task<List<SpeciesSearchResult>> SearchSpeciesAsync(string query, string? kingdom = null)
    {
        _logger.LogDebug("Searching cached species for query: {Query}, kingdom: {Kingdom}", query, kingdom ?? "all");

        // Check cache first
        if (EcoStore.SearchCache.TryGetValue(query, out var cachedResults))
        {
            var cachedResultsWithScores = cachedResults
                .Select(cache => new
                {
                    Species = EcoStore.Species.TryGetValue(cache.SpeciesId, out var species) ? species : null,
                    cache.RelevanceScore
                })
                .Where(x => x.Species != null)
                .Select(x => new
                {
                    x.Species!.ScientificName,
                    x.Species.Kingdom,
                    VernacularNames = EcoStore.TaxonCommonNames.Values
                        .Where(tcn => tcn.SpeciesId == x.Species.Id)
                        .Select(tcn => tcn.CommonName)
                        .ToList(),
                    Score = x.RelevanceScore
                })
                .ToList();

            if (cachedResultsWithScores.Count != 0)
            {
                _logger.LogDebug("Cache hit for query: {Query} with {Count} results", query, cachedResultsWithScores.Count);

                // Filter by kingdom if specified
                var filteredByKingdom = cachedResultsWithScores;
                if (!string.IsNullOrWhiteSpace(kingdom) && !kingdom.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    filteredByKingdom = cachedResultsWithScores
                        .Where(x => x.Kingdom != null && x.Kingdom.Equals(kingdom, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    _logger.LogDebug("Filtered cache results to {Count} for kingdom: {Kingdom}", filteredByKingdom.Count, kingdom);
                }

                // Get images for results
                var results = new List<SpeciesSearchResult>();
                foreach (var item in filteredByKingdom.OrderByDescending(x => x.Score))
                {
                    var imageUrl = await GetSpeciesImageAsync(item.ScientificName);
                    results.Add(new SpeciesSearchResult
                    {
                        ScientificName = item.ScientificName,
                        Kingdom = item.Kingdom,
                        VernacularNames = item.VernacularNames,
                        ImageUrl = imageUrl
                    });
                }

                return results;
            }
        }

        _logger.LogDebug("Cache miss for query: {Query}", query);
        return [];
    }

    public async Task<string?> GetSpeciesImageAsync(string scientificName)
    {
        // Check cache first
        if (EcoStore.SpeciesImages.TryGetValue(scientificName, out var cachedImage))
        {
            return cachedImage.ImageUrl;
        }

        // Fetch image from iNaturalist via client
        var imageUrl = await _iNaturalistClient.GetSpeciesImageAsync(scientificName);

        // Cache the result (null if no photo found)
        var speciesImage = new SpeciesImage
        {
            ScientificName = scientificName,
            ImageUrl = imageUrl,
            CachedAt = DateTime.UtcNow
        };
        EcoStore.SpeciesImages[scientificName] = speciesImage;

        _logger.LogDebug("Cached image for species: {ScientificName}, URL: {ImageUrl}", scientificName, imageUrl ?? "none");
        return imageUrl;
    }

    public Task<int> StoreSpeciesAsync(Species species)
    {
        var speciesId = species.Id == 0 ? EcoStore.GetNextSpeciesId() : species.Id;
        species.Id = speciesId;
        EcoStore.Species[speciesId] = species;
        _logger.LogDebug("Stored species: {SpeciesId} - {ScientificName}", speciesId, species.ScientificName);
        return Task.FromResult(speciesId);
    }

    public Task StoreCommonNameAsync(int speciesId, string commonName, string source)
    {
        var commonNameId = Guid.NewGuid();
        var taxonCommonName = new TaxonCommonName
        {
            Id = commonNameId,
            SpeciesId = speciesId,
            CommonName = commonName,
            Source = source
        };
        EcoStore.TaxonCommonNames[commonNameId] = taxonCommonName;
        _logger.LogDebug("Stored common name for species {SpeciesId}: {CommonName}", speciesId, commonName);
        return Task.CompletedTask;
    }

    public Task StoreSearchCacheAsync(string query, int speciesId, double relevanceScore)
    {
        var searchCacheEntry = new SpeciesCache
        {
            QueryText = query,
            SpeciesId = speciesId,
            RelevanceScore = relevanceScore,
            CreatedAt = DateTime.UtcNow
        };

        if (!EcoStore.SearchCache.ContainsKey(query))
            EcoStore.SearchCache[query] = [];

        EcoStore.SearchCache[query].Add(searchCacheEntry);
        _logger.LogDebug("Stored search cache entry for query: {Query}, species: {SpeciesId}, score: {Score}",
            query, speciesId, relevanceScore);
        return Task.CompletedTask;
    }
}
