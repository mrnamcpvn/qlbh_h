using API._Repositories;
using API._Services.Interfaces;
using API._Services.Services;

namespace API.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void AddDependencyInjectionConfiguration(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Add RepositoryAccessor
            services.AddScoped<IRepositoryAccessor, RepositoryAccessor>();

            // Add Service
            services.AddScoped<I_Auth, S_Auth>();
            services.AddScoped<I_CalculateWages, S_CalculateWages>();
            services.AddScoped<I_CommodityCode, S_CommodityCode>();
            services.AddScoped<I_Employee, S_Employee>();
            services.AddScoped<I_StepInProcess, S_StepInProcess>();
            services.AddScoped<I_User, S_User>();
            services.AddScoped<I_Report, S_Report>();
        }
    }
}