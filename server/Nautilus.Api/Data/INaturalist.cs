using System.Text.Json.Serialization;

namespace Nautilus.Api.Data;

/// <summary>
/// DTOs for iNaturalist API responses.
/// </summary>

/// <summary>
/// Root response object from iNaturalist taxa search API.
/// </summary>
internal class INaturalistResult
{
    [JsonPropertyName("total_results")]
    public int TotalResults { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("results")]
    public List<INaturalistTaxon> Results { get; set; } = [];
}

/// <summary>
/// Represents a taxon entry from iNaturalist with associated photos.
/// </summary>
internal class INaturalistTaxon
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("rank")]
    public string? Rank { get; set; }

    [JsonPropertyName("preferred_common_name")]
    public string? PreferredCommonName { get; set; }

    [JsonPropertyName("default_photo")]
    public INaturalistPhoto? DefaultPhoto { get; set; }

    [JsonPropertyName("taxon_photos")]
    public List<INaturalistTaxonPhoto>? TaxonPhotos { get; set; }
}

/// <summary>
/// Represents a photo from iNaturalist API.
/// </summary>
internal class INaturalistPhoto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("square_url")]
    public string? SquareUrl { get; set; }

    [JsonPropertyName("medium_url")]
    public string? MediumUrl { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

/// <summary>
/// Wrapper for taxon photo from iNaturalist API.
/// </summary>
internal class INaturalistTaxonPhoto
{
    [JsonPropertyName("photo")]
    public INaturalistPhoto? Photo { get; set; }
}
