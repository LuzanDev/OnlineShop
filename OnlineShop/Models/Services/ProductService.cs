using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache _cache;
        private readonly string _productsCacheKey = "Products_All";

        public ProductService(IBaseRepository<Product> productRepository, IMemoryCache productsCache)
        {
            _productRepository = productRepository;
            _cache = productsCache;
        }

        public async Task<CollectionResult<Product>> GetAllProducts()
        {
            if (!_cache.TryGetValue(_productsCacheKey, out List<Product> products))
            {
                products = await LoadProducts();
                if (products.Count < 1)
                {
                    return new CollectionResult<Product>()
                    {
                        ErrorMessage = "Products not found",
                        ErrorCode = (int)ErrorCodes.ProductCollectionNotFound,
                    };
                }

                _cache.Set(_productsCacheKey, products, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });

            }
            return new CollectionResult<Product>()
            {
                Data = products,
                Count = products.Count
            };
        }
        public async Task<BaseResult<Product>> DeleteProduct(long id)
        {
            if (!_cache.TryGetValue(_productsCacheKey, out List<Product> products))
            {
                products = await LoadProducts();
            }
            var productRemoved = products.FirstOrDefault(x => x.Id == id);

            if (productRemoved == null)
            {
                return new BaseResult<Product>()
                {
                    ErrorMessage = "ProductNotFound",
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                };
            }

            await _productRepository.Delete(productRemoved);
            products.Remove(productRemoved);

            _cache.Set(_productsCacheKey, products, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });


            return new BaseResult<Product>()
            {
                Data = productRemoved
            };
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
            product = await _productRepository.AddAsync(product);

            if (_cache.TryGetValue(_productsCacheKey, out List<Product> products))
            {
                if (!products.Contains(product))
                {
                    product.Images = product.Images.OrderBy(iproduct => iproduct.Order).ToList();
                    products.Add(product);

                    _cache.Set(_productsCacheKey, products, new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(30)
                    });
                }
            }
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
            if (!_cache.TryGetValue(_productsCacheKey, out List<Product> products))
            {
                products = await LoadProducts();
            }
            var productUpdated = products.FirstOrDefault(x => x.Id == id);

            if (productUpdated == null)
            {
                return new BaseResult<Product>()
                {
                    ErrorCode = (int)ErrorCodes.ProductNotFound,
                    ErrorMessage = "ProductNotFound"
                };
            }

            productUpdated.Name = model.Name;
            productUpdated.Description = model.Description;
            productUpdated.Price = model.Price;
            productUpdated.CategoryId = model.CategoryId;
            productUpdated.BrandId = model.BrandId;

            List<int> orderIndexes = model.ImageOrder.Split(',').Select(int.Parse).ToList();
            List<ProductImage> sortedImages = new List<ProductImage>();

            for (int i = 0; i < productUpdated.Images.Count; i++)
            {
                sortedImages.Add(productUpdated.Images[orderIndexes[i]]);
                sortedImages[i].Order = i;
            }

            productUpdated.Images = sortedImages;

            await _productRepository.Update(productUpdated);

            _cache.Set(_productsCacheKey, products, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });

            return new BaseResult<Product>()
            {
                Data = productUpdated,
            };
        }
        public async Task<BaseResult<Product>> GetProductById(long id)
        {
            if (!_cache.TryGetValue(_productsCacheKey, out List<Product> products))
            {
                products = await LoadProducts();

                _cache.Set(_productsCacheKey, products, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }
            var product = products.FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                return new BaseResult<Product>()
                {
                    ErrorMessage = "ProductNotFound",
                    ErrorCode = (int)ErrorCodes.ProductNotFound
                };
            }
            return new BaseResult<Product>()
            {
                Data = product,
            };
        }
        public async Task<CollectionResult<Product>> GetProductsByListId(List<long> listId)
        {
            if (listId.Count > 0)
            {
                if (!_cache.TryGetValue(_productsCacheKey, out List<Product> products))
                {
                    products = await LoadProducts();

                    _cache.Set(_productsCacheKey, products, new MemoryCacheEntryOptions()
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(30)
                    });
                }
                var productsListIds = products.Where(p => listId.Contains(p.Id)).ToList();

                return new CollectionResult<Product>()
                {
                    Data = productsListIds
                };
            }

            return new CollectionResult<Product>()
            {
                ErrorCode = (int)ErrorCodes.ProductCollectionNotFound,
                ErrorMessage = "ProductCollectionNotFound",
            };
        }
        public async Task<CollectionResult<Product>> GetProductsByCategoryId(int id)
        {
            if (!_cache.TryGetValue(_productsCacheKey, out List<Product> products))
            {
                products = await LoadProducts();

                _cache.Set(_productsCacheKey, products, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }
            var listProductsByCategoryId = products.Where(x => x.CategoryId == id).ToList();


            if (listProductsByCategoryId.Count < 1)
            {
                return new CollectionResult<Product>()
                {
                    ErrorMessage = "ProductCollectionNotFound",
                    ErrorCode = (int)ErrorCodes.ProductCollectionNotFound,
                };
            }
            return new CollectionResult<Product>()
            {
                Data = listProductsByCategoryId,
                Count = listProductsByCategoryId.Count
            };
        }
        private async Task<List<Product>> LoadProducts()
        {
            var products = await _productRepository.GetAll()
                .Include(x => x.Images)
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .ToListAsync();

            foreach (var item in products)
            {
                item.Images = item.Images.OrderBy(image => image.Order).ToList();
            }
            return products;
        }
    }
}
