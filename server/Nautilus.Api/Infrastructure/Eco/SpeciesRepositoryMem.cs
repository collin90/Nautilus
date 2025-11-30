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

    // INATURALIST API constants

    private const string _inatBaseUrlV1 = "https://api.inaturalist.org/v1";
    private const string _inatTaxaSearchEndpoint = "/taxa";

    // Relevance filtering
    private const double _minRelevanceScore = 100.0; // Minimum score to include in results

    // JSON deserialization options
    private readonly JsonSerializerOptions _jsonSerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc/>
    public async Task<List<SpeciesSearchResult>> SearchSpeciesAsync(string query, string? kingdom = null)
    {
        try
        {
            // 1. Check cache first
            logger.LogDebug("Searching species for query: {Query}, kingdom: {Kingdom}", query, kingdom ?? "all");
            if (EcoStoreMem.SearchCache.TryGetValue(query, out var cachedResults))
            {
                var cachedResultsWithScores = cachedResults
                    .Select(cache => new
                    {
                        Species = EcoStoreMem.Species.TryGetValue(cache.SpeciesId, out var species) ? species : null,
                        cache.RelevanceScore
                    })
                    .Where(x => x.Species != null)
                    .Select(x => new
                    {
                        x.Species!.ScientificName,
                        x.Species.Kingdom,
                        VernacularNames = EcoStoreMem.TaxonCommonNames.Values
                            .Where(tcn => tcn.SpeciesId == x.Species.Id)
                            .Select(tcn => tcn.CommonName)
                            .ToList(),
                        Score = x.RelevanceScore
                    })
                    .ToList();

                if (cachedResultsWithScores.Count != 0)
                {
                    logger.LogDebug("Cache hit for query: {Query} with {Count} results", query, cachedResultsWithScores.Count);

                    // Filter by kingdom if specified
                    var filteredByKingdom = cachedResultsWithScores;
                    if (!string.IsNullOrWhiteSpace(kingdom) && !kingdom.Equals("all", StringComparison.OrdinalIgnoreCase))
                    {
                        filteredByKingdom = cachedResultsWithScores
                            .Where(x => x.Kingdom != null && x.Kingdom.Equals(kingdom, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        logger.LogDebug("Filtered cache results to {Count} for kingdom: {Kingdom}", filteredByKingdom.Count, kingdom);
                    }

                    // Apply filtering and get images
                    var results = new List<SpeciesSearchResult>();
                    foreach (var item in filteredByKingdom.Where(x => x.Score >= _minRelevanceScore)
                        .OrderByDescending(x => x.Score))
                    {
                        var imageUrl = await GetSpeciesImagesAsync(item.ScientificName);
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
            logger.LogDebug("Cache miss for query: {Query}", query);

            // 2. Cache miss - call GBIF API with increased limit
            GbifResult gbifResult = await GetGbifSpeciesResult(query);

            // 3. Filter by kingdom if specified (GBIF API doesn't respect kingdom parameter)
            if (!string.IsNullOrWhiteSpace(kingdom) && !kingdom.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                gbifResult.Results = [.. gbifResult.Results.Where(s => s.Kingdom != null && s.Kingdom.Equals(kingdom, StringComparison.OrdinalIgnoreCase))];
                logger.LogDebug("Filtered to {Count} results for kingdom: {Kingdom}", gbifResult.Results.Count, kingdom);
            }

            // 4. Preprocess and deduplicate GBIF data
            var processedSpecies = GbifSpecies.PreprocessGbifSpecies(gbifResult.Results);
            logger.LogDebug("Processing {Count} unique species for query: {Query}", processedSpecies.Count, query);

            // 5. Build species results and populate cache
            var speciesResults = await BuildSpeciesResultsAndCache(processedSpecies, query);

            // 6. Filter and sort by relevance
            var filteredResults = FilterAndSortByRelevance(speciesResults, query);

            logger.LogDebug("Search completed for query: {Query}, found {Count} unique species", query, filteredResults.Count);
            return filteredResults;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching species for query: {Query}", query);
            return [];
        }
    }

    /// <summary>
    /// Calls GBIF species search API and returns results.
    /// </summary>
    /// <param name="query"> The search query text entered by the user. </param>
    /// <returns> GBIF species search results. </returns>
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

    /// <summary>
    /// Fetches species image from iNaturalist API and caches it.
    /// Returns the default photo if available, otherwise the first available photo from any result.
    /// </summary>
    private async Task<string?> GetSpeciesImagesAsync(string scientificName)
    {
        // Check cache first
        if (EcoStoreMem.SpeciesImages.TryGetValue(scientificName, out var cachedImage))
        {
            return cachedImage.ImageUrl;
        }

        try
        {
            var inatUrl = $"{_inatBaseUrlV1}{_inatTaxaSearchEndpoint}?q={Uri.EscapeDataString(scientificName)}";
            var response = await _httpClient.GetAsync(inatUrl);

            if (!response.IsSuccessStatusCode)
                return null;

            var jsonContent = await response.Content.ReadAsStringAsync();
            var inatResult = JsonSerializer.Deserialize<INaturalistResult>(jsonContent, _jsonSerOptions);

            if (inatResult?.Results == null || inatResult.Results.Count == 0)
                return null;

            // Iterate through results to find one with a valid photo, preferring default_photo
            string? imageUrl = null;
            foreach (var taxon in inatResult.Results)
            {
                // Prefer default photo if available
                if (!string.IsNullOrWhiteSpace(taxon.DefaultPhoto?.MediumUrl))
                {
                    imageUrl = taxon.DefaultPhoto.MediumUrl;
                    break;
                }

                // Fallback to first taxon photo if no default photo
                if (taxon.TaxonPhotos != null && taxon.TaxonPhotos.Count > 0)
                {
                    var firstPhoto = taxon.TaxonPhotos[0].Photo?.MediumUrl;
                    if (!string.IsNullOrWhiteSpace(firstPhoto))
                    {
                        imageUrl = firstPhoto;
                        break;
                    }
                }
            }

            // Cache the result (null if no photo found)
            var speciesImage = new SpeciesImage
            {
                ScientificName = scientificName,
                ImageUrl = imageUrl,
                CachedAt = DateTime.UtcNow
            };
            EcoStoreMem.SpeciesImages[scientificName] = speciesImage;

            logger.LogDebug("Cached image for species: {ScientificName}, URL: {ImageUrl}", scientificName, imageUrl ?? "none");
            return imageUrl;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Error fetching images for species: {ScientificName}", scientificName);
            return null;
        }
    }

    /// <summary>
    /// Filters species search results and sorts them by relevance to the search query.
    /// Relevance is based on: 1) Scientific name match, 2) Vernacular name match, 3) Similarity.
    /// </summary>
    private static List<SpeciesSearchResult> FilterAndSortByRelevance(List<SpeciesSearchResult> results, string query)
    {
        var normalizedQuery = query.Trim().ToLowerInvariant();

        // Calculate scores for all results by creating temporary TaxonomicTree objects
        var scoresDict = results.ToDictionary(
            r => r,
            r =>
            {
                var tempTree = new TaxonomicTree { Species = r.ScientificName, VernacularNames = r.VernacularNames };
                return SpeciesRelevanceUtil.CalculateRelevanceScore(tempTree, normalizedQuery);
            }
        );

        // Filter by minimum relevance score and sort
        return [.. results
            .Where(r => scoresDict.TryGetValue(r, out var score) && score >= _minRelevanceScore)
            .OrderByDescending(r => scoresDict.TryGetValue(r, out var score) ? score : 0)
            .ThenBy(r => r.ScientificName)];
    }

    /// <summary>
    /// Builds species search results from preprocessed GBIF species and populates cache.
    /// </summary>
    private async Task<List<SpeciesSearchResult>> BuildSpeciesResultsAndCache(List<GbifSpecies> gbifSpeciesList, string query)
    {
        var results = new List<SpeciesSearchResult>();
        var normalizedQuery = query.Trim().ToLowerInvariant();

        foreach (var gbifSpecies in gbifSpeciesList)
        {
            var scientificName = gbifSpecies.ScientificName ?? "Unknown";
            logger.LogDebug("Processing species: Key={Key}, ScientificName={ScientificName}", gbifSpecies.Key, scientificName);

            // Fetch vernacular names from GBIF species detail endpoint
            var vernacularNames = await FetchVernacularNamesAsync(gbifSpecies.Key);

            // Store species
            var speciesId = EcoStoreMem.GetNextSpeciesId();
            var species = new Species
            {
                Id = speciesId,
                ScientificName = scientificName,
                Kingdom = gbifSpecies.Kingdom,
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

            // Fetch image from iNaturalist
            var imageUrl = await GetSpeciesImagesAsync(scientificName);

            // Create result object
            var result = new SpeciesSearchResult
            {
                ScientificName = scientificName,
                Kingdom = gbifSpecies.Kingdom,
                VernacularNames = vernacularNames,
                ImageUrl = imageUrl
            };
            results.Add(result);

            // Calculate and store relevance score in cache (using temporary tree for scoring)
            var tempTree = new TaxonomicTree
            {
                Species = scientificName,
                VernacularNames = vernacularNames
            };
            var relevanceScore = SpeciesRelevanceUtil.CalculateRelevanceScore(tempTree, normalizedQuery);

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

        return results;
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

}
