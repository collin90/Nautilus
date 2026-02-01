using Nautilus.Api.Services;
using Nautilus.Api.Services.Email;
using Nautilus.Api.Services.Security;

namespace Nautilus.Api.Configuration;

public static class ServiceConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Core utilities
        services.AddSingleton<JwtService>();

        // Email service with HttpClient
        services.AddHttpClient<IEmailService, EmailService>();

        // Business logic services (single implementation that uses backends)
        services.AddScoped<IAuthService, Services.Auth.AuthService>();
        services.AddScoped<IProfileService, Services.Profile.ProfileService>();
        services.AddScoped<ISpeciesSearchService, Services.Eco.SpeciesSearchService>();
        services.AddScoped<ITaxonomicDataService, Services.Eco.TaxonomicDataService>();

        return services;
    }
}
