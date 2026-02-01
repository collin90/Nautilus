using Nautilus.Api.Backend;
using Nautilus.Api.Backend.Mock;
using Nautilus.Api.Backend.Sql;
using MockAuth = Nautilus.Api.Backend.Mock.AuthBackend;
using MockProfile = Nautilus.Api.Backend.Mock.ProfileBackend;
using MockSpecies = Nautilus.Api.Backend.Mock.SpeciesBackend;
using MockTaxonomic = Nautilus.Api.Backend.Mock.TaxonomicBackend;
using SqlAuth = Nautilus.Api.Backend.Sql.AuthBackend;
using SqlProfile = Nautilus.Api.Backend.Sql.ProfileBackend;
using SqlSpecies = Nautilus.Api.Backend.Sql.SpeciesBackend;
using SqlTaxonomic = Nautilus.Api.Backend.Sql.TaxonomicBackend;

namespace Nautilus.Api.Configuration;

public static class BackendConfiguration
{
    public static IServiceCollection AddBackends(this IServiceCollection services, IConfiguration configuration)
    {
        var backendType = configuration.GetValue("Backend:Type", "memory");

        if (backendType == "memory")
        {
            // Mock backends (in-memory with caching)
            services.AddSingleton<IAuthBackend, MockAuth>();
            services.AddSingleton<IProfileBackend, MockProfile>();
            services.AddSingleton<ISpeciesBackend, MockSpecies>();
            services.AddSingleton<ITaxonomicBackend, MockTaxonomic>();
        }
        else
        {
            // Database backends  
            services.AddScoped<IAuthBackend, SqlAuth>();
            services.AddScoped<IProfileBackend, SqlProfile>();
            services.AddScoped<ISpeciesBackend, SqlSpecies>();
            services.AddScoped<ITaxonomicBackend, SqlTaxonomic>();
        }

        return services;
    }
}
