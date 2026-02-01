using Nautilus.Api.Data;

namespace Nautilus.Api.Backend;

/// <summary>
/// Backend interface for species search and data operations
/// </summary>
public interface ISpeciesBackend
{
    /// <summary>
    /// Search for species by query and optional kingdom filter
    /// </summary>
    Task<List<SpeciesSearchResult>> SearchSpeciesAsync(string query, string? kingdom = null);

    /// <summary>
    /// Get species image by scientific name
    /// </summary>
    Task<string?> GetSpeciesImageAsync(string scientificName);

    /// <summary>
    /// Store a species and return its ID
    /// </summary>
    Task<int> StoreSpeciesAsync(Species species);

    /// <summary>
    /// Store a common name for a species
    /// </summary>
    Task StoreCommonNameAsync(int speciesId, string commonName, string source);

    /// <summary>
    /// Store a search cache entry
    /// </summary>
    Task StoreSearchCacheAsync(string query, int speciesId, double relevanceScore);
}
