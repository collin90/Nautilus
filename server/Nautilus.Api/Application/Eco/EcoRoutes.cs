using Nautilus.Api.Application.Eco.Species;

namespace Nautilus.Api.Application.Eco;

public static class EcoRoutes
{
    public static IEndpointRouteBuilder MapEcoRoutes(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/species").WithTags("Species");

        group.MapGet("", SpeciesHandler.Handle);

        return routes;
    }
}
