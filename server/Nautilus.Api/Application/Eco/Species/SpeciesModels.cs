namespace Nautilus.Api.Application.Eco.Species;

public class SpeciesSearchResponse
{
    public List<TaxonomicTree> Results { get; set; } = new();
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
    public List<string> VernacularNames { get; set; } = new();
}
