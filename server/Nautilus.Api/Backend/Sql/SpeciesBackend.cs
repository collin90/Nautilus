using Nautilus.Api.Data;

namespace Nautilus.Api.Backend.Sql;

/// <summary>
/// Database implementation of species backend (TODO: Implement)
/// </summary>
public class SpeciesBackend(ILogger<SpeciesBackend> logger) : ISpeciesBackend
{
    private readonly ILogger<SpeciesBackend> _logger = logger;

    public async Task<List<SpeciesSearchResult>> SearchSpeciesAsync(string query, string? kingdom = null)
    {
        // TODO: Implement database query with full-text search
        _logger.LogWarning("SpeciesBackend.SearchSpeciesAsync not implemented - using placeholder");
        await Task.Delay(10);
        return [];
    }

    public async Task<string?> GetSpeciesImageAsync(string scientificName)
    {
        // TODO: Implement database query for cached species images
        _logger.LogWarning("SpeciesBackend.GetSpeciesImageAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return null;
    }

    public async Task<int> StoreSpeciesAsync(Species species)
    {
        // TODO: Implement database insert/update
        _logger.LogWarning("SpeciesBackend.StoreSpeciesAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return 0;
    }

    public async Task StoreCommonNameAsync(int speciesId, string commonName, string source)
    {
        // TODO: Implement database insert
        _logger.LogWarning("SpeciesBackend.StoreCommonNameAsync not implemented - using placeholder");
        await Task.CompletedTask;
    }

    public async Task StoreSearchCacheAsync(string query, int speciesId, double relevanceScore)
    {
        // TODO: Implement database insert for search cache
        _logger.LogWarning("SpeciesBackend.StoreSearchCacheAsync not implemented - using placeholder");
        await Task.CompletedTask;
    }
}
