using Nautilus.Api.Data;

namespace Nautilus.Api.Backend.Sql;

/// <summary>
/// Database implementation of taxonomic backend (TODO: Implement)
/// </summary>
public class TaxonomicBackend(ILogger<TaxonomicBackend> logger) : ITaxonomicBackend
{
    private readonly ILogger<TaxonomicBackend> _logger = logger;

    public TaxonomicTree BuildTaxonomicTree(Species species)
    {
        // TODO: Implement database queries to build taxonomic tree
        _logger.LogWarning("TaxonomicBackend.BuildTaxonomicTree not implemented - returning empty tree");
        return new TaxonomicTree
        {
            Species = species.ScientificName,
            VernacularNames = []
        };
    }

    public async Task<int> GetOrCreateKingdomAsync(string? name)
    {
        // TODO: Implement database query/insert
        _logger.LogWarning("TaxonomicBackend.GetOrCreateKingdomAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return 0;
    }

    public async Task<int> GetOrCreatePhylumAsync(string? name, int kingdomId)
    {
        // TODO: Implement database query/insert
        _logger.LogWarning("TaxonomicBackend.GetOrCreatePhylumAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return 0;
    }

    public async Task<int> GetOrCreateClassAsync(string? name, int phylumId)
    {
        // TODO: Implement database query/insert
        _logger.LogWarning("TaxonomicBackend.GetOrCreateClassAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return 0;
    }

    public async Task<int> GetOrCreateOrderAsync(string? name, int classId)
    {
        // TODO: Implement database query/insert
        _logger.LogWarning("TaxonomicBackend.GetOrCreateOrderAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return 0;
    }

    public async Task<int> GetOrCreateFamilyAsync(string? name, int orderId)
    {
        // TODO: Implement database query/insert
        _logger.LogWarning("TaxonomicBackend.GetOrCreateFamilyAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return 0;
    }

    public async Task<int> GetOrCreateGenusAsync(string? name, int familyId)
    {
        // TODO: Implement database query/insert
        _logger.LogWarning("TaxonomicBackend.GetOrCreateGenusAsync not implemented - using placeholder");
        await Task.CompletedTask;
        return 0;
    }
}
