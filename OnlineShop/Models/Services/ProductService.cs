using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;
using OnlineShop.Views.ViewModel;

namespace OnlineShop.Models.Services
{
    public class ProductService : IProductService
    {
        private readonly IBaseRepository<Product> _productRepository;

        public ProductService(IBaseRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public  async Task<BaseResult<Product>> CreateProductAsync(ProductViewModel model)
        {
            Product product = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                Images = ConvertFilesToImages(model.Images),
            };
           await _productRepository.AddAsync(product);
            return new BaseResult<Product>()
            {
                Data = product
            };
        }

        public async Task<CollectionResult<Product>> GetAllProducts()
        {
            // возможные ошибки
            var products = await _productRepository.GetAll()
                .Include(x =>x.Images)
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .ToListAsync();
            if (!products.Any())
            {
                return new CollectionResult<Product>()
                {
                    ErrorMessage = "Products not found", // Reurces
                    ErrorCode = (int)ErrorCodes.ProductsNotFound,
                };
            }
            else
            {
                return new CollectionResult<Product>()
                {
                    Data = products,
                    Count = products.Count
                };
            }
        }

        private List<ProductImage> ConvertFilesToImages(List<IFormFile> images)
        {
            List<ProductImage> productImages = new List<ProductImage>();

            foreach (var image in images)
            {
                ProductImage productImage = new ProductImage();
                productImage.FileName = image.FileName;

                // Считываем данные изображения в массив байтов
                using (var stream = new MemoryStream())
                {
                    image.CopyTo(stream);
                    productImage.Data = stream.ToArray();
                }

                productImages.Add(productImage);
            }
            return productImages;
        }
    }
}
