using Nautilus.Api.Services;
using Nautilus.Api.Data;

namespace Nautilus.Api.Application.Eco.Species;

public static class SpeciesHandler
{
    public static async Task<IResult> Handle(string? query, string? kingdom, ISpeciesSearchService repo)
    {
        if (string.IsNullOrWhiteSpace(query))
            return Results.BadRequest("Query parameter is required");

        try
        {
            var results = await repo.SearchSpeciesAsync(query, kingdom);
            return Results.Ok(new SpeciesSearchResponse { Results = results });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error processing species search: {ex.Message}");
        }
    }
}
