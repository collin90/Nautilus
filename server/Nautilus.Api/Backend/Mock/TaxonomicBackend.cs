using Nautilus.Api.Data;

namespace Nautilus.Api.Backend.Mock;

/// <summary>
/// In-memory implementation of taxonomic backend using EcoStore
/// </summary>
public class TaxonomicBackend : ITaxonomicBackend
{
    /// <inheritdoc/>
    public TaxonomicTree BuildTaxonomicTree(Species species)
    {
        // Traverse hierarchy to build full taxonomic tree
        var genus = EcoStore.Genuses.TryGetValue(species.GenusId, out var g) ? g : null;
        var family = genus != null && EcoStore.Families.TryGetValue(genus.FamilyId, out var f) ? f : null;
        var order = family != null && EcoStore.Orders.TryGetValue(family.OrderId, out var o) ? o : null;
        var classEntity = order != null && EcoStore.Classes.TryGetValue(order.ClassId, out var c) ? c : null;
        var phylum = classEntity != null && EcoStore.Phylums.TryGetValue(classEntity.PhylumId, out var p) ? p : null;
        var kingdom = phylum != null && EcoStore.Kingdoms.TryGetValue(phylum.KingdomId, out var k) ? k : null;

        var vernacularNames = EcoStore.TaxonCommonNames.Values
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

    /// <inheritdoc/>
    public Task<int> GetOrCreateKingdomAsync(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStore.Kingdoms.Values.FirstOrDefault(k => k.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (existing != null) return Task.FromResult(existing.Id);

        var id = EcoStore.GetNextKingdomId();
        EcoStore.Kingdoms[id] = new Kingdom { Id = id, Name = name };
        return Task.FromResult(id);
    }

    /// <inheritdoc/>
    public Task<int> GetOrCreatePhylumAsync(string? name, int kingdomId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStore.Phylums.Values.FirstOrDefault(p =>
            p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && p.KingdomId == kingdomId);
        if (existing != null) return Task.FromResult(existing.Id);

        var id = EcoStore.GetNextPhylumId();
        EcoStore.Phylums[id] = new Phylum { Id = id, Name = name, KingdomId = kingdomId };
        return Task.FromResult(id);
    }

    /// <inheritdoc/>
    public Task<int> GetOrCreateClassAsync(string? name, int phylumId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStore.Classes.Values.FirstOrDefault(c =>
            c.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && c.PhylumId == phylumId);
        if (existing != null) return Task.FromResult(existing.Id);

        var id = EcoStore.GetNextClassId();
        EcoStore.Classes[id] = new Class { Id = id, Name = name, PhylumId = phylumId };
        return Task.FromResult(id);
    }

    /// <inheritdoc/>
    public Task<int> GetOrCreateOrderAsync(string? name, int classId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStore.Orders.Values.FirstOrDefault(o =>
            o.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && o.ClassId == classId);
        if (existing != null) return Task.FromResult(existing.Id);

        var id = EcoStore.GetNextOrderId();
        EcoStore.Orders[id] = new Order { Id = id, Name = name, ClassId = classId };
        return Task.FromResult(id);
    }

    /// <inheritdoc/>
    public Task<int> GetOrCreateFamilyAsync(string? name, int orderId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStore.Families.Values.FirstOrDefault(f =>
            f.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && f.OrderId == orderId);
        if (existing != null) return Task.FromResult(existing.Id);

        var id = EcoStore.GetNextFamilyId();
        EcoStore.Families[id] = new Family { Id = id, Name = name, OrderId = orderId };
        return Task.FromResult(id);
    }

    /// <inheritdoc/>
    public Task<int> GetOrCreateGenusAsync(string? name, int familyId)
    {
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";

        var existing = EcoStore.Genuses.Values.FirstOrDefault(g =>
            g.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && g.FamilyId == familyId);
        if (existing != null) return Task.FromResult(existing.Id);

        var id = EcoStore.GetNextGenusId();
        EcoStore.Genuses[id] = new Genus { Id = id, Name = name, FamilyId = familyId };
        return Task.FromResult(id);
    }
}
