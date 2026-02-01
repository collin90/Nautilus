using Nautilus.Api.Application.Eco.Species;
using Nautilus.Api.Data;

namespace Nautilus.Api.Services;

/// <summary>
/// Service for accessing and building taxonomic data hierarchies
/// </summary>
public interface ITaxonomicDataService
{
    /// <summary>
    /// Builds a complete taxonomic tree from a species entity
    /// </summary>
    /// <param name="species">The species to build the tree for</param>
    /// <returns>A complete taxonomic tree with all hierarchy levels</returns>
    TaxonomicTree BuildTaxonomicTree(Species species);

    /// <summary>
    /// Gets or creates a kingdom by name
    /// </summary>
    Task<int> GetOrCreateKingdomAsync(string? name);

    /// <summary>
    /// Gets or creates a phylum by name and kingdom
    /// </summary>
    Task<int> GetOrCreatePhylumAsync(string? name, int kingdomId);

    /// <summary>
    /// Gets or creates a class by name and phylum
    /// </summary>
    Task<int> GetOrCreateClassAsync(string? name, int phylumId);

    /// <summary>
    /// Gets or creates an order by name and class
    /// </summary>
    Task<int> GetOrCreateOrderAsync(string? name, int classId);

    /// <summary>
    /// Gets or creates a family by name and order
    /// </summary>
    Task<int> GetOrCreateFamilyAsync(string? name, int orderId);

    /// <summary>
    /// Gets or creates a genus by name and family
    /// </summary>
    Task<int> GetOrCreateGenusAsync(string? name, int familyId);
}
