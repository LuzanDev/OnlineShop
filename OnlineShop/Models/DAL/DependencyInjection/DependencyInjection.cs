using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.DAL.Repositories;
using OnlineShop.Models.Email;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Identity;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Models.Services;

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

            services.AddAuthentication()
            .AddGoogle(options =>
            {
                IConfigurationSection googleAuthNSection = configuration.GetSection("Authentication:Google");
                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
                options.CallbackPath = "/signin-google";
            });

            services.AddDefaultIdentity<ApplicationUser>(options =>
                options.SignIn.RequireConfirmedEmail = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.InitRepositories();
        }



        private static void InitRepositories(this IServiceCollection services)
        {
            //repositories
            services.AddScoped<IBaseRepository<Product>, BaseRepository<Product>>();
            services.AddScoped<IBaseRepository<Brand>, BaseRepository<Brand>>();
            services.AddScoped<IBaseRepository<Category>, BaseRepository<Category>>();
            services.AddScoped<IBaseRepository<FavoriteProduct>, BaseRepository<FavoriteProduct>>();
            services.AddScoped<IBaseRepository<Cart>, BaseRepository<Cart>>();
            services.AddScoped<IBaseRepository<City>, BaseRepository<City>>();
            services.AddScoped<IBaseRepository<Order>, BaseRepository<Order>>();




            //services
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddTransient<IEmailSender, SendGridEmailSender>();

        }
    }
}
