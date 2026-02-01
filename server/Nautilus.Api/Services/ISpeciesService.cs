using Nautilus.Api.Application.Eco.Species;

using Nautilus.Api.Data;

namespace Nautilus.Api.Services;

public interface ISpeciesSearchService
{
    ///<summary>
    /// Search species by query text (user input) and optional kingdom filter
    ///</summary>
    /// <param name="query"> The search query text entered by the user. </param>
    /// <param name="kingdom"> Optional kingdom filter to narrow down the search results. </param>
    /// <returns> A list of species search results matching the query and filter criteria. </returns>
    Task<List<SpeciesSearchResult>> SearchSpeciesAsync(string query, string? kingdom = null);

    /// <summary>
    /// Fetches a species image by scientific name (with caching)
    /// </summary>
    /// <param name="scientificName">The scientific name of the species</param>
    /// <returns>The image URL if found, otherwise null</returns>
    Task<string?> GetSpeciesImageAsync(string scientificName);
}
