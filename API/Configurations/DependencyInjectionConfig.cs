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
            services.AddScoped<I_DonHang, S_DonHang>();
            services.AddScoped<I_CommodityCode, S_CommodityCode>();
            services.AddScoped<I_KhachHang, S_KhachHang>();
            services.AddScoped<I_SanPham, S_SanPham>();
            services.AddScoped<I_User, S_User>();
            services.AddScoped<I_Report, S_Report>();
        }
    }
}