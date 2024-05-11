using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Services
{
    public class BrandService : IBrandService
    {
       private readonly IBaseRepository<Brand> _brandRepository;

        public BrandService(IBaseRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }

        public async Task<bool> BrandAlreadExists(string nameBrand)
        {
            var brands = await _brandRepository.GetAll().Select(x => x.Name).ToListAsync();
            return brands.Contains(nameBrand);
        }

        public async Task<BaseResult<Brand>> CreateBrandAsync(string nameBrand)
        {
            Brand brand = new Brand()
            {
                Name = nameBrand
            };
           brand =  await _brandRepository.AddAsync(brand);
            return new BaseResult<Brand>()
            {
                Data = brand
            };
        }

        public async Task<BaseResult<Brand>> DeleteBrand(int id)
        {
            var brand = await _brandRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (brand == null)
            {
                return new BaseResult<Brand>()
                {
                    ErrorMessage = "BrandNotFound",
                    ErrorCode = (int)ErrorCodes.BrandNotFound
                };
            }
            else
            {
               await _brandRepository.Delete(brand);
                return new BaseResult<Brand>()
                {
                    Data = brand
                };
            }
        }

        public async Task<CollectionResult<Brand>> GetAllBrands()
        {
            var brands = await _brandRepository.GetAll().ToListAsync();
            if (!brands.Any())
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
                Brand brand = new Brand()
                {
                    Id  = brandId,
                    Name = newBrandName
                };
                await _brandRepository.Update(brand);
                return new BaseResult<Brand>()
                {
                    Data = brand
                };
            
        }
    }
}
