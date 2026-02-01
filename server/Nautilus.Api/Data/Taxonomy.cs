namespace Nautilus.Api.Data;

public class Kingdom
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}

public class Phylum
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int KingdomId { get; set; }
}

public class Class
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int PhylumId { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int ClassId { get; set; }
}

public class Family
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int OrderId { get; set; }
}

public class Genus
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public int FamilyId { get; set; }
}

public class Species
{
    public int Id { get; set; }
    public string ScientificName { get; set; } = default!;
    public string? Kingdom { get; set; }
    public int GenusId { get; set; }
    public long? UsageKey { get; set; }
    public string? Authorship { get; set; }
    public string? GbifLink { get; set; }
}

public class TaxonCommonName
{
    public Guid Id { get; set; }
    public int SpeciesId { get; set; }
    public string CommonName { get; set; } = default!;
    public string? Language { get; set; }
    public string? Source { get; set; }
    public bool IsPreferred { get; set; }
}


public class SpeciesImage
{
    public string ScientificName { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public DateTime CachedAt { get; set; }
}

