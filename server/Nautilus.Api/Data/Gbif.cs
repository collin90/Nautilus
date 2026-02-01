namespace Nautilus.Api.Data;

/// <summary>
/// DTOs for GBIF (Global Biodiversity Information Facility) API responses.
/// </summary>
/// 
/// <summary>
/// Root response object from GBIF species search API.
/// </summary>
public class GbifResult
{
    public int Offset { get; set; }
    public int Limit { get; set; }
    public bool EndOfRecords { get; set; }
    public List<GbifSpecies> Results { get; set; } = [];
}

/// <summary>
/// Represents a species entry from GBIF API with taxonomic hierarchy.
/// </summary>
public class GbifSpecies
{
    public int Key { get; set; }
    public string? ScientificName { get; set; }
    public string? CanonicalName { get; set; }
    public string? Kingdom { get; set; }
    public string? Phylum { get; set; }
    public string? Class { get; set; }
    public string? Order { get; set; }
    public string? Family { get; set; }
    public string? Genus { get; set; }
    public string? Species { get; set; }
    // GBIF returns vernacularNames as objects with vernacularName and language properties
    public List<GbifVernacularName>? VernacularNames { get; set; }

}

/// <summary>
/// Root response object from GBIF vernacular names API.
/// </summary>
public class GbifVernacularResult
{
    public List<GbifVernacularName> Results { get; set; } = [];
}

/// <summary>
/// Represents a vernacular (common) name from GBIF with language information.
/// </summary>
public class GbifVernacularName
{
    public string? VernacularName { get; set; }
    public string? Language { get; set; }
}

