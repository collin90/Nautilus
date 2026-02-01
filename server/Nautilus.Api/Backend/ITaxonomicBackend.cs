using Nautilus.Api.Data;

namespace Nautilus.Api.Backend;

/// <summary>
/// Backend interface for taxonomic data operations
/// </summary>
public interface ITaxonomicBackend
{
    TaxonomicTree BuildTaxonomicTree(Species species);
    Task<int> GetOrCreateKingdomAsync(string? name);
    Task<int> GetOrCreatePhylumAsync(string? name, int kingdomId);
    Task<int> GetOrCreateClassAsync(string? name, int phylumId);
    Task<int> GetOrCreateOrderAsync(string? name, int classId);
    Task<int> GetOrCreateFamilyAsync(string? name, int orderId);
    Task<int> GetOrCreateGenusAsync(string? name, int familyId);
}
