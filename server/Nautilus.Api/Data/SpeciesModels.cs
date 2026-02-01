namespace Nautilus.Api.Data;

public class SpeciesSearchResponse
{
    public List<SpeciesSearchResult> Results { get; set; } = [];
}

public class SpeciesSearchResult
{
    public string ScientificName { get; set; } = string.Empty;
    public string? Kingdom { get; set; }
    public List<string> VernacularNames { get; set; } = [];
    public string? ImageUrl { get; set; }
}

public class TaxonomicTree
{
    public string? Kingdom { get; set; }
    public string? Phylum { get; set; }
    public string? Class { get; set; }
    public string? Order { get; set; }
    public string? Family { get; set; }
    public string? Genus { get; set; }
    public string? Species { get; set; }
    public List<string> VernacularNames { get; set; } = [];
}
