namespace Nautilus.Api.Clients;

/// <summary>
/// Client for interacting with the iNaturalist API
/// </summary>
public interface IINaturalistClient
{
    /// <summary>
    /// Searches for a taxon by scientific name and returns the first available image URL
    /// </summary>
    /// <param name="scientificName">The scientific name to search for</param>
    /// <returns>The image URL if found, otherwise null</returns>
    Task<string?> GetSpeciesImageAsync(string scientificName);
}
