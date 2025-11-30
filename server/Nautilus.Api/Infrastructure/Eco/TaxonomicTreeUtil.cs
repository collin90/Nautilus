using Nautilus.Api.Application.Eco.Species;

namespace Nautilus.Api.Infrastructure.Eco;

/// <summary>
/// Utility class for building complete taxonomic trees from cached species data.
/// </summary>
public static class TaxonomicTreeUtil
{
    /// <summary>
    /// Builds a complete taxonomic tree from a cached Species entity.
    /// </summary>
    public static TaxonomicTree BuildTaxonomicTree(Species species)
    {
        // Traverse hierarchy to build full taxonomic tree
        var genus = EcoStoreMem.Genuses.TryGetValue(species.GenusId, out var g) ? g : null;
        var family = genus != null && EcoStoreMem.Families.TryGetValue(genus.FamilyId, out var f) ? f : null;
        var order = family != null && EcoStoreMem.Orders.TryGetValue(family.OrderId, out var o) ? o : null;
        var classEntity = order != null && EcoStoreMem.Classes.TryGetValue(order.ClassId, out var c) ? c : null;
        var phylum = classEntity != null && EcoStoreMem.Phylums.TryGetValue(classEntity.PhylumId, out var p) ? p : null;
        var kingdom = phylum != null && EcoStoreMem.Kingdoms.TryGetValue(phylum.KingdomId, out var k) ? k : null;

        var vernacularNames = EcoStoreMem.TaxonCommonNames.Values
            .Where(tcn => tcn.SpeciesId == species.Id)
            .Select(tcn => tcn.CommonName)
            .ToList();

        return new TaxonomicTree
        {
            Kingdom = kingdom?.Name,
            Phylum = phylum?.Name,
            Class = classEntity?.Name,
            Order = order?.Name,
            Family = family?.Name,
            Genus = genus?.Name,
            Species = species.ScientificName,
            VernacularNames = vernacularNames
        };
    }


    /// <summary>
    /// Applies kingdom filtering and sorting using pre-calculated relevance scores.
    /// </summary>
    private static List<TaxonomicTree> ApplyKingdomFilterAndSort(
        List<TaxonomicTree> trees,
        Dictionary<TaxonomicTree, double> relevanceScores,
        double minRelevanceScore,
        string? kingdom)
    {
        // Normalize kingdom filter
        var kingdomFilter = kingdom?.Trim().ToLowerInvariant();

        // Filter by kingdom if specified
        var filtered = trees;
        if (!string.IsNullOrWhiteSpace(kingdomFilter) && kingdomFilter != "all")
        {
            filtered = [.. trees.Where(t => t.Kingdom != null && t.Kingdom.Equals(kingdom, StringComparison.OrdinalIgnoreCase))];
        }

        // Filter by minimum relevance score and sort
        return [.. filtered
            .Where(t => relevanceScores.TryGetValue(t, out var score) && score >= minRelevanceScore)
            .OrderByDescending(t => relevanceScores.TryGetValue(t, out var score) ? score : 0)
            .ThenBy(t => GetKingdomSortOrder(t.Kingdom))
            .ThenBy(t => t.Species)];
    }

    /// <summary>
    /// Returns sort order for kingdoms (lower number = higher priority).
    /// </summary>
    private static int GetKingdomSortOrder(string? kingdom)
    {
        return kingdom?.ToLowerInvariant() switch
        {
            "animalia" => 1,
            "plantae" => 2,
            "fungi" => 3,
            "bacteria" => 4,
            "archaea" => 5,
            "protista" => 6,
            "chromista" => 7,
            _ => 99 // Unknown kingdoms go last
        };
    }


    private static int GetOrCreateKingdom(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Kingdoms.Values.FirstOrDefault(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextKingdomId();
        EcoStoreMem.Kingdoms[id] = new Kingdom { Id = id, Name = name };
        return id;
    }

    private static int GetOrCreatePhylum(string? name, int kingdomId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Phylums.Values.FirstOrDefault(p =>
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && p.KingdomId == kingdomId);
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextPhylumId();
        EcoStoreMem.Phylums[id] = new Phylum { Id = id, Name = name, KingdomId = kingdomId };
        return id;
    }

    private static int GetOrCreateClass(string? name, int phylumId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Classes.Values.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.PhylumId == phylumId);
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextClassId();
        EcoStoreMem.Classes[id] = new Class { Id = id, Name = name, PhylumId = phylumId };
        return id;
    }

    private static int GetOrCreateOrder(string? name, int classId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Orders.Values.FirstOrDefault(o =>
            o.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && o.ClassId == classId);
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextOrderId();
        EcoStoreMem.Orders[id] = new Order { Id = id, Name = name, ClassId = classId };
        return id;
    }

    private static int GetOrCreateFamily(string? name, int orderId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Families.Values.FirstOrDefault(f =>
            f.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && f.OrderId == orderId);
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextFamilyId();
        EcoStoreMem.Families[id] = new Family { Id = id, Name = name, OrderId = orderId };
        return id;
    }

    private static int GetOrCreateGenus(string? name, int familyId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStoreMem.Genuses.Values.FirstOrDefault(g =>
            g.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && g.FamilyId == familyId);
        if (existing != null) return existing.Id;

        var id = EcoStoreMem.GetNextGenusId();
        EcoStoreMem.Genuses[id] = new Genus { Id = id, Name = name, FamilyId = familyId };
        return id;
    }
}
