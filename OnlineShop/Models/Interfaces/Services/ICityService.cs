using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface ICityService
    {
        Task<CollectionResult<City>> GetAllCity();
        Task<BaseResult<City>> GetCityByName(string name);
    }
}
