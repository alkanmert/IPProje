using IPProje.Extensions.Repositories;
using IPProje.Extensions.UnitOfWorks;
using IPProje.Models;
using Microsoft.EntityFrameworkCore;

namespace IPProje.Extensions
{
    public static class DataExtensions
    {
        public static IServiceCollection LoadDataLayerExtension(this IServiceCollection services, IConfiguration config) 
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
