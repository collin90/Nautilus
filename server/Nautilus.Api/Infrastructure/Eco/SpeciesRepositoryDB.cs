using Nautilus.Api.Application.Eco.Species;

namespace Nautilus.Api.Infrastructure.Eco;

public class SpeciesRepositoryDB : ISpeciesRepository
{
    // Inject your database client/service here
    public SpeciesRepositoryDB()
    {
        // Initialize DB connection if needed
    }

    public async Task<List<TaxonomicTree>> SearchSpeciesAsync(string query, string? kingdom = null)
    {
        // TODO: Implement actual DB query logic
        await Task.Delay(10);
        return [];
    }
}
