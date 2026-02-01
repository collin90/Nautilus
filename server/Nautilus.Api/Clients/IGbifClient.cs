using Nautilus.Api.Data;

namespace Nautilus.Api.Clients;

/// <summary>
/// Client for interacting with the GBIF (Global Biodiversity Information Facility) API
/// </summary>
public interface IGbifClient
{
    /// <summary>
    /// Searches for species in the GBIF database
    /// </summary>
    /// <param name="query">The search query text</param>
    /// <param name="limit">Maximum number of results to return (default: 100)</param>
    /// <param name="rank">Taxonomic rank to filter by (default: SPECIES)</param>
    /// <param name="status">Taxonomic status to filter by (default: ACCEPTED)</param>
    /// <returns>GBIF search results containing species data</returns>
    Task<GbifResult> SearchSpeciesAsync(string query, int limit = 100, string rank = "SPECIES", string status = "ACCEPTED");

    /// <summary>
    /// Fetches vernacular (common) names for a species by its GBIF key
    /// </summary>
    /// <param name="speciesKey">The GBIF species key</param>
    /// <returns>List of vernacular names</returns>
    Task<List<string>> GetVernacularNamesAsync(int speciesKey);
}
