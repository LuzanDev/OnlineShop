using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Entity;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Interfaces.Services
{
    public interface IBrandService
    {
        Task<CollectionResult<Brand>> GetAllBrands();
        Task<BaseResult<Brand>> CreateBrandAsync(string brandName);
        Task<BaseResult<Brand>> UpdateBrand(int brandId, string newBrandName);
        Task<BaseResult<Brand>> DeleteBrand(int id);
        Task<bool> BrandAlreadExists(string brandName);
    }
}
