using Nautilus.Api.Data;

namespace Nautilus.Api.Backend.Mock;

/// <summary>
/// Shared in-memory store for ecological data in mock backend implementations
/// </summary>
public static class EcoStore
{
    public static Dictionary<int, Kingdom> Kingdoms { get; } = new();
    public static Dictionary<int, Phylum> Phylums { get; } = new();
    public static Dictionary<int, Class> Classes { get; } = new();
    public static Dictionary<int, Order> Orders { get; } = new();
    public static Dictionary<int, Family> Families { get; } = new();
    public static Dictionary<int, Genus> Genuses { get; } = new();
    public static Dictionary<int, Species> Species { get; } = new();
    public static Dictionary<Guid, TaxonCommonName> TaxonCommonNames { get; } = new();
    public static Dictionary<string, List<SpeciesCache>> SearchCache { get; } = new(StringComparer.OrdinalIgnoreCase);
    public static Dictionary<string, SpeciesImage> SpeciesImages { get; } = new(StringComparer.OrdinalIgnoreCase);

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
