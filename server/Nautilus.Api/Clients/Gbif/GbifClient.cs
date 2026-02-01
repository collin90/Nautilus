using System.Text.Json;
using Nautilus.Api.Data;

namespace Nautilus.Api.Clients.Gbif;

/// <summary>
/// Implementation of GBIF API client
/// </summary>
public class GbifClient : IGbifClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GbifClient> _logger;
    private readonly JsonSerializerOptions _jsonSerOptions;

    private const string BaseUrl = "https://api.gbif.org/v1";
    private const string SpeciesSearchEndpoint = "/species/search";
    private const string VernacularNamesEndpointTemplate = "/species/{0}/vernacularNames";

    public GbifClient(HttpClient httpClient, ILogger<GbifClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _jsonSerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc/>
    public async Task<GbifResult> SearchSpeciesAsync(string query, int limit = 100, string rank = "SPECIES", string status = "ACCEPTED")
    {
        try
        {
            var gbifUrl = $"{BaseUrl}{SpeciesSearchEndpoint}?q={Uri.EscapeDataString(query)}&rank={rank}&status={status}&limit={limit}";
            var response = await _httpClient.GetAsync(gbifUrl);
            
            _logger.LogDebug("Called GBIF API for query: {Query}", query);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GBIF API call failed with status code: {StatusCode}", response.StatusCode);
                return new GbifResult();
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var gbifResult = JsonSerializer.Deserialize<GbifResult>(jsonContent, _jsonSerOptions);

            if (gbifResult == null || gbifResult.Results == null)
            {
                _logger.LogWarning("GBIF API returned null or invalid data for query: {Query}", query);
                return new GbifResult();
            }

            _logger.LogDebug("GBIF API returned {Count} results for query: {Query}", gbifResult.Results.Count, query);
            return gbifResult;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling GBIF API for query: {Query}", query);
            return new GbifResult();
        }
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetVernacularNamesAsync(int speciesKey)
    {
        try
        {
            var vernacularUrl = $"{BaseUrl}{string.Format(VernacularNamesEndpointTemplate, speciesKey)}";
            var response = await _httpClient.GetAsync(vernacularUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogDebug("GBIF vernacular names API call failed with status code: {StatusCode} for species key: {SpeciesKey}", 
                    response.StatusCode, speciesKey);
                return [];
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            var vernacularResult = JsonSerializer.Deserialize<GbifVernacularResult>(jsonContent, _jsonSerOptions);

            if (vernacularResult?.Results == null)
            {
                _logger.LogDebug("GBIF vernacular names API returned null or invalid data for species key: {SpeciesKey}", speciesKey);
                return [];
            }

            var names = vernacularResult.Results
                .Where(v => !string.IsNullOrWhiteSpace(v.VernacularName))
                .Select(v => v.VernacularName!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            _logger.LogDebug("Retrieved {Count} vernacular names for species key: {SpeciesKey}", names.Count, speciesKey);
            return names;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching vernacular names for species key: {SpeciesKey}", speciesKey);
            return [];
        }
    }
}
