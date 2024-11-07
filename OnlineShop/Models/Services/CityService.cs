using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;

namespace OnlineShop.Models.Services
{
    public class CityService : ICityService
    {
        private readonly IBaseRepository<City> _cityRepository;

        public CityService(IBaseRepository<City> cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<CollectionResult<City>> GetAllCity()
        {
            var cities = await _cityRepository.GetAll()
                .OrderBy(city => city.Name)
                .ToListAsync();
            if (cities == null || !cities.Any())
            {
                return new CollectionResult<City>()
                {
                    ErrorMessage = "CityCollectionNotFound",
                    ErrorCode = (int)ErrorCodes.CityCollectionNotFound
                };
            }
            return new CollectionResult<City>()
            {
                Data = cities,
                Count = cities.Count
            };
        }

        public async Task<BaseResult<City>> GetCityByName(string name)
        {
            var city = await _cityRepository.GetAll()
                .Where(x => x.Name == name)
                .FirstOrDefaultAsync();

            if (city == null)
            {
                return new BaseResult<City>()
                {
                    ErrorMessage = "CityNotFound",
                    ErrorCode = (int)ErrorCodes.CityNotFound
                };
            }
            return new BaseResult<City>()
            {
                Data = city
            };
        }
    }
}
