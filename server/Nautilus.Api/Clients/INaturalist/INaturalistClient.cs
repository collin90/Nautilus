using System.Text.Json;
using Nautilus.Api.Data;

namespace Nautilus.Api.Clients.INaturalist;

/// <summary>
/// Implementation of iNaturalist API client
/// </summary>
public class INaturalistClient : IINaturalistClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<INaturalistClient> _logger;
    private readonly JsonSerializerOptions _jsonSerOptions;

    private const string BaseUrl = "https://api.inaturalist.org/v1";
    private const string TaxaSearchEndpoint = "/taxa";

    public INaturalistClient(HttpClient httpClient, ILogger<INaturalistClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonSerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc/>
    public async Task<string?> GetSpeciesImageAsync(string scientificName)
    {
        try
        {
            var inatUrl = $"{BaseUrl}{TaxaSearchEndpoint}?q={Uri.EscapeDataString(scientificName)}";
            var response = await _httpClient.GetAsync(inatUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug("iNaturalist API call failed with status code: {StatusCode} for species: {ScientificName}",
                    response.StatusCode, scientificName);
                return null;
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var inatResult = JsonSerializer.Deserialize<INaturalistResult>(jsonContent, _jsonSerOptions);

            if (inatResult?.Results == null || inatResult.Results.Count == 0)
            {
                _logger.LogDebug("iNaturalist API returned no results for species: {ScientificName}", scientificName);
                return null;
            }

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

            if (imageUrl != null)
            {
                _logger.LogDebug("Retrieved image for species: {ScientificName}, URL: {ImageUrl}", scientificName, imageUrl);
            }
            else
            {
                _logger.LogDebug("No image found for species: {ScientificName}", scientificName);
            }

            return imageUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching image from iNaturalist for species: {ScientificName}", scientificName);
            return null;
        }
    }
}
