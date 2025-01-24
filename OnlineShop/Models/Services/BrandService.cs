using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;

namespace OnlineShop.Models.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBaseRepository<Brand> _brandRepository;
        private readonly IMemoryCache _cache;
        private readonly string _brandsCacheKey = "Brands_All";

        public BrandService(IBaseRepository<Brand> brandRepository, IMemoryCache cache)
        {
            _brandRepository = brandRepository;
            _cache = cache;
        }

        public async Task<bool> BrandAlreadExists(string nameBrand)
        {
            if (!_cache.TryGetValue(_brandsCacheKey, out List<Brand> brands))
            {
                brands = await LoadBrands();

                _cache.Set(_brandsCacheKey, brands, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }
            var listNameBrands = brands.Select(x => x.Name).ToList();

            return listNameBrands.Contains(nameBrand);
        }
        public async Task<BaseResult<Brand>> CreateBrandAsync(string nameBrand)
        {
            if (!_cache.TryGetValue(_brandsCacheKey, out List<Brand> brands))
            {
                brands = await LoadBrands();
            }

            Brand brand = new Brand()
            {
                Name = nameBrand
            };

            brand = await _brandRepository.AddAsync(brand);
            brands.Add(brand);

            _cache.Set(_brandsCacheKey, brands, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });

            return new BaseResult<Brand>()
            {
                Data = brand
            };
        }
        public async Task<BaseResult<Brand>> DeleteBrand(int id)
        {
            if (!_cache.TryGetValue(_brandsCacheKey, out List<Brand> brands))
            {
                brands = await LoadBrands();
            }

            var brandRemoved = brands.FirstOrDefault(x => x.Id == id);
            if (brandRemoved == null)
            {
                return new BaseResult<Brand>()
                {
                    ErrorMessage = "BrandNotFound",
                    ErrorCode = (int)ErrorCodes.BrandNotFound
                };
            }

            await _brandRepository.Delete(brandRemoved);
            brands.Remove(brandRemoved);

            _cache.Set(_brandsCacheKey, brands, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });

            return new BaseResult<Brand>()
            {
                Data = brandRemoved
            };
        }
        public async Task<CollectionResult<Brand>> GetAllBrands()
        {
            if (!_cache.TryGetValue(_brandsCacheKey, out List<Brand> brands))
            {
                brands = await LoadBrands();

                _cache.Set(_brandsCacheKey, brands, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }

            if (brands.Count < 1)
            {
                return new CollectionResult<Brand>()
                {
                    ErrorMessage = "BrandCollectionNotFound",
                    ErrorCode = (int)ErrorCodes.BrandCollectionNotFound
                };
            }
            else
            {
                return new CollectionResult<Brand>()
                {
                    Data = brands,
                    Count = brands.Count
                };
            }
        }
        public async Task<BaseResult<Brand>> UpdateBrand(int brandId, string newBrandName)
        {
            if (!_cache.TryGetValue(_brandsCacheKey, out List<Brand> brands))
            {
                brands = await LoadBrands();

                _cache.Set(_brandsCacheKey, brands, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }
            var brandUpdated = brands.FirstOrDefault(x => x.Id == brandId);

            if (brandUpdated == null)
            {
                return new BaseResult<Brand>()
                {
                    ErrorMessage = "BrandNotFound",
                    ErrorCode = (int)ErrorCodes.BrandNotFound
                };
            }

            brandUpdated.Name = newBrandName;

            await _brandRepository.Update(brandUpdated);

            return new BaseResult<Brand>()
            {
                Data = brandUpdated
            };
        }
        private async Task<List<Brand>> LoadBrands()
        {
            var brands = await _brandRepository.GetAll().ToListAsync();

            return brands;
        }
    }
}
