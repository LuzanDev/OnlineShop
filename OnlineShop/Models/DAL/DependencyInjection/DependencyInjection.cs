using Microsoft.EntityFrameworkCore;

namespace OnlineShop.Models.DAL.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options => 
            {
                options.UseNpgsql(configuration.GetConnectionString("PostgresSQL"));
            });

        }
    }
}
