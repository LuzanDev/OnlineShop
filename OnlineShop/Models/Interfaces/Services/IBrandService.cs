using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Entity;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface IBrandService
    {
        Task<CollectionResult<Brand>> GetAllBrands();
        Task<BaseResult<Brand>> CreateBrandAsync(BrandViewModel model);
        Task<BaseResult<Brand>> UpdateBrand(Brand brand);
        Task<BaseResult<Brand>> DeleteBrand(int id);
    }
}
