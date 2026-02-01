using Nautilus.Api.Data;
using Nautilus.Api.Backend;

namespace Nautilus.Api.Services.Eco;

/// <summary>
/// Taxonomic data service that delegates to backend implementation
/// </summary>
public class TaxonomicDataService(ITaxonomicBackend backend) : ITaxonomicDataService
{
    private readonly ITaxonomicBackend _backend = backend;

    public TaxonomicTree BuildTaxonomicTree(Species species)
    {
        return _backend.BuildTaxonomicTree(species);
    }

    public Task<int> GetOrCreateKingdomAsync(string? name)
    {
        return _backend.GetOrCreateKingdomAsync(name);
    }

    public Task<int> GetOrCreatePhylumAsync(string? name, int kingdomId)
    {
        return _backend.GetOrCreatePhylumAsync(name, kingdomId);
    }

    public Task<int> GetOrCreateClassAsync(string? name, int phylumId)
    {
        return _backend.GetOrCreateClassAsync(name, phylumId);
    }

    public Task<int> GetOrCreateOrderAsync(string? name, int classId)
    {
        return _backend.GetOrCreateOrderAsync(name, classId);
    }

    public Task<int> GetOrCreateFamilyAsync(string? name, int orderId)
    {
        return _backend.GetOrCreateFamilyAsync(name, orderId);
    }

    public Task<int> GetOrCreateGenusAsync(string? name, int familyId)
    {
        return _backend.GetOrCreateGenusAsync(name, familyId);
    }
}
