using Nautilus.Api.Application.Eco.Species;

namespace Nautilus.Api.Infrastructure.Eco;

public interface ISpeciesRepository
{
    ///<summary>
    /// Search species by query text (user input) and optional kingdom filter
    ///</summary>
    /// <param name="query"> The search query text entered by the user. </param>
    /// <param name="kingdom"> Optional kingdom filter to narrow down the search results. </param>
    /// <returns> A list of species search results matching the query and filter criteria. </returns>
    Task<List<SpeciesSearchResult>> SearchSpeciesAsync(string query, string? kingdom = null);
}
