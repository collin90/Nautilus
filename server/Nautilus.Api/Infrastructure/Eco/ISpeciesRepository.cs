using Nautilus.Api.Application.Eco.Species;

namespace Nautilus.Api.Infrastructure.Eco;

public interface ISpeciesRepository
{
    Task<List<TaxonomicTree>> SearchSpeciesAsync(string query, string? kingdom = null);
}
