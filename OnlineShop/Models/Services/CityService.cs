using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;
        private readonly string _citiesCacheKey = "Cities_All";

        public CityService(IBaseRepository<City> cityRepository, IMemoryCache cache)
        {
            _cityRepository = cityRepository;
            _cache = cache;
        }

        public async Task<CollectionResult<City>> GetAllCity()
        {
            if (!_cache.TryGetValue(_citiesCacheKey, out List<City> cities))
            {
                cities = await LoadCities();

                _cache.Set(_citiesCacheKey, cities, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }
            
            if (cities == null || cities.Count < 1)
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
            if (!_cache.TryGetValue(_citiesCacheKey, out List<City> cities))
            {
                cities = await LoadCities();

                _cache.Set(_citiesCacheKey, cities, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }

            var city = cities.FirstOrDefault(x => x.Name == name);

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

        private async Task<List<City>> LoadCities()
        {
            var cities = await _cityRepository.GetAll().OrderBy(city => city.Name).ToListAsync();
            return cities;
        }
    }
}
