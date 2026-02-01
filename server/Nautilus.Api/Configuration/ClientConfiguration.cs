using Nautilus.Api.Clients;
using Nautilus.Api.Clients.Gbif;
using Nautilus.Api.Clients.INaturalist;

namespace Nautilus.Api.Configuration;

public static class ClientConfiguration
{
    public static IServiceCollection AddExternalClients(this IServiceCollection services)
    {
        // Configure HttpClient for GbifClient
        services.AddHttpClient<IGbifClient, GbifClient>();

        // Configure HttpClient for INaturalistClient
        services.AddHttpClient<IINaturalistClient, INaturalistClient>();

        return services;
    }
}
