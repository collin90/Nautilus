namespace Nautilus.Api.Data;

public class SpeciesCache
{
    public string QueryText { get; set; } = default!;
    public int SpeciesId { get; set; }
    public double RelevanceScore { get; set; }
    public DateTime CreatedAt { get; set; }
}
