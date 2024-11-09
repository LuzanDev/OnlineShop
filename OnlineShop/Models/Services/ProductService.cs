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



        public async Task<CollectionResult<Product>> GetAllProducts()
        {
            var products = await _productRepository.GetAll()
                .Include(x => x.Images)
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .ToListAsync();
            if (!products.Any())
            {
                return new CollectionResult<Product>()
                {
                    ErrorMessage = "Products not found", // Reurces
                    ErrorCode = (int)ErrorCodes.ProductCollectionNotFound,
                };
            }
            else
            {
                foreach (var product in products)
                {
                    product.Images = product.Images.OrderBy(image => image.Order).ToList();
                }

                return new CollectionResult<Product>()
                {
                    Data = products,
                    Count = products.Count
                };
            }
        }

        public async Task<BaseResult<Product>> DeleteProduct(long id)
        {
            var product = await _productRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return new BaseResult<Product>()
                {
                    ErrorMessage = "ProductNotFound",
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                };
            }
            else
            {
                await _productRepository.Delete(product);
                return new BaseResult<Product>()
                {
                    Data = product
                };
            }
        }

        public async Task<BaseResult<Product>> CreateProductAsync(ProductViewModel model)
        {
            List<int> imageOrder = new List<int>();
            if (model.ImageOrder == null)
            {
                imageOrder = Enumerable.Range(0, model.Images.Count).ToList();
            }
            else
            {
                imageOrder = model.ImageOrder.Split(',').Select(int.Parse).ToList();
            }


            var images = ConvertFilesToImages(model.Images);
            List<ProductImage> sortImage = new List<ProductImage>();
            for (int i = 0; i < model.Images.Count; i++)
            {
                sortImage.Add(images[imageOrder[i]]);
                sortImage[i].Order = i;
            }


            Product product = new Product()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                Images = sortImage
            };
            await _productRepository.AddAsync(product);
            return new BaseResult<Product>()
            {
                Data = product
            };
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

        public async Task<BaseResult<Product>> UpdateProduct(long id, ProductViewModel model)
        {
            var product = await _productRepository.GetAll()
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            product.Images = product.Images.OrderBy(image => image.Order).ToList();


            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;
            product.BrandId = model.BrandId;

            List<int> orderIndexes = model.ImageOrder.Split(',').Select(int.Parse).ToList();
            List<ProductImage> sortedImages = new List<ProductImage>();

            for (int i = 0; i < product.Images.Count; i++)
            {
                sortedImages.Add(product.Images[orderIndexes[i]]);
                sortedImages[i].Order = i;
            }

            product.Images = sortedImages;

            await _productRepository.Update(product);
            return new BaseResult<Product>()
            {
                Data = product,
            };

        }

        public async Task<BaseResult<Product>> GetProductById(long id)
        {
            var product = await _productRepository.GetAll()
                .Include(x => x.Images)
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return new BaseResult<Product>()
                {
                    ErrorMessage = "ProductNotFound",
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                };
            }
            else
            {
                product.Images = product.Images.OrderBy(image => image.Order).ToList();
                return new BaseResult<Product>()
                {
                    Data = product,
                };
            }

        }

        public async Task<CollectionResult<Product>> GetProductsByListId(List<long> listId)
        {
            if (listId.Count > 0)
            {
                var products = await _productRepository.GetAll()
                .Include(x => x.Images)
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .Where(p => listId.Contains(p.Id))
                .ToListAsync();

                return new CollectionResult<Product>()
                {
                    Data = products
                };
            }

            return new CollectionResult<Product>()
            {
                ErrorCode = (int)ErrorCodes.ProductNotFound,
                ErrorMessage = "ProductNotFound",
            };
        }

        public async Task<CollectionResult<Product>> GetProductsByCategoryId(int id)
        {
            var products = await _productRepository.GetAll()
                .Include(x => x.Images)
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .Where(x => x.CategoryId == id)
                .ToListAsync();
            if (!products.Any())
            {
                return new CollectionResult<Product>()
                {
                    ErrorMessage = "Products not found", 
                    ErrorCode = (int)ErrorCodes.ProductCollectionNotFound,
                };
            }
            else
            {
                foreach (var product in products)
                {
                    product.Images = product.Images.OrderBy(image => image.Order).ToList();
                }

                return new CollectionResult<Product>()
                {
                    Data = products,
                    Count = products.Count
                };
            }
        }
    }
}
