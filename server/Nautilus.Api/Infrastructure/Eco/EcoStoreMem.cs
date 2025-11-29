namespace Nautilus.Api.Infrastructure.Eco;

// In-memory cache models mirroring the SQL schema
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

public class SearchCache
{
    public string QueryText { get; set; } = default!;
    public int SpeciesId { get; set; }
    public decimal Confidence { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Static in-memory store for ecological data
public static class EcoStoreMem
{
    public static Dictionary<int, Kingdom> Kingdoms { get; } = new();
    public static Dictionary<int, Phylum> Phylums { get; } = new();
    public static Dictionary<int, Class> Classes { get; } = new();
    public static Dictionary<int, Order> Orders { get; } = new();
    public static Dictionary<int, Family> Families { get; } = new();
    public static Dictionary<int, Genus> Genuses { get; } = new();
    public static Dictionary<int, Species> Species { get; } = new();
    public static Dictionary<Guid, TaxonCommonName> TaxonCommonNames { get; } = new();
    public static Dictionary<string, List<SearchCache>> SearchCache { get; } = new(StringComparer.OrdinalIgnoreCase);

    private static int _nextKingdomId = 1;
    private static int _nextPhylumId = 1;
    private static int _nextClassId = 1;
    private static int _nextOrderId = 1;
    private static int _nextFamilyId = 1;
    private static int _nextGenusId = 1;
    private static int _nextSpeciesId = 1;

    public static int GetNextKingdomId() => _nextKingdomId++;
    public static int GetNextPhylumId() => _nextPhylumId++;
    public static int GetNextClassId() => _nextClassId++;
    public static int GetNextOrderId() => _nextOrderId++;
    public static int GetNextFamilyId() => _nextFamilyId++;
    public static int GetNextGenusId() => _nextGenusId++;
    public static int GetNextSpeciesId() => _nextSpeciesId++;
}
