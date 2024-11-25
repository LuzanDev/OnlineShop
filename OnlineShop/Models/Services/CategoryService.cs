using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OnlineShop.Models.Entity;
using OnlineShop.Models.Entity.Result;
using OnlineShop.Models.Enums;
using OnlineShop.Models.Interfaces.Repository;
using OnlineShop.Models.Interfaces.Services;

namespace OnlineShop.Models.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IBaseRepository<Category> _categoryRepository;
        private readonly IMemoryCache _cache;
        private readonly string _categoriesCacheKey = "Categories_All";

        public CategoryService(IBaseRepository<Category> categoryRepository, IMemoryCache cache)
        {
            _categoryRepository = categoryRepository;
            _cache = cache;
        }

        public async Task<bool> CategoryAlreadExists(string categoryName)
        {
            if (!_cache.TryGetValue(_categoriesCacheKey, out List<Category> categories))
            {
                categories = await LoadCategories();

                _cache.Set(_categoriesCacheKey, categories, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }
            var listNameCategories = categories.Select(x => x.Name).ToList();

            return listNameCategories.Contains(categoryName);
        }
        public async Task<BaseResult<Category>> CreateCategoryAsync(string nameCategory)
        {
            if (!_cache.TryGetValue(_categoriesCacheKey, out List<Category> categories))
            {
                categories = await LoadCategories();
            }

            Category category = new Category()
            {
                Name = nameCategory
            };

            category = await _categoryRepository.AddAsync(category);
            categories.Add(category);

            _cache.Set(_categoriesCacheKey, categories, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });

            return new BaseResult<Category>()
            {
                Data = category
            };
        }
        public async Task<BaseResult<Category>> DeleteCategory(int id)
        {
            if (!_cache.TryGetValue(_categoriesCacheKey, out List<Category> categories))
            {
                categories = await LoadCategories();
            }

            var categoryRemoved = categories.FirstOrDefault(x => x.Id == id);
            if (categoryRemoved == null)
            {
                return new BaseResult<Category>()
                {
                    ErrorMessage = "CategoryNotFound",
                    ErrorCode = (int)ErrorCodes.CategoryNotFound
                };
            }

            await _categoryRepository.Delete(categoryRemoved);
            categories.Remove(categoryRemoved);

            _cache.Set(_categoriesCacheKey, categories, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromMinutes(30)
            });

            return new BaseResult<Category>()
            {
                Data = categoryRemoved
            };
        }
        public async Task<CollectionResult<Category>> GetAllCategory()
        {
            if (!_cache.TryGetValue(_categoriesCacheKey, out List<Category> categories))
            {
                categories = await LoadCategories();

                _cache.Set(_categoriesCacheKey, categories, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }

            if (categories.Count < 1)
            {
                return new CollectionResult<Category>()
                {
                    ErrorMessage = "CategoryCollectionNotFound",
                    ErrorCode = (int)ErrorCodes.CategoryCollectionNotFound
                };
            }
            return new CollectionResult<Category>()
            {
                Data = categories,
                Count = categories.Count
            };

        }
        public async Task<BaseResult<Category>> UpdateCategory(int categoryId, string newNameCategory)
        {
            if (!_cache.TryGetValue(_categoriesCacheKey, out List<Category> categories))
            {
                categories = await LoadCategories();

                _cache.Set(_categoriesCacheKey, categories, new MemoryCacheEntryOptions()
                {
                    SlidingExpiration = TimeSpan.FromMinutes(30)
                });
            }
            var categoryUpdated = categories.FirstOrDefault(c => c.Id == categoryId);

            if (categoryUpdated == null)
            {
                return new BaseResult<Category>()
                {
                    ErrorMessage = "CategoryNotFound",
                    ErrorCode = (int)ErrorCodes.CategoryNotFound
                };
            }

            categoryUpdated.Name = newNameCategory;

            await _categoryRepository.Update(categoryUpdated);

            return new BaseResult<Category>()
            {
                Data = categoryUpdated
            };
        }
        private async Task<List<Category>> LoadCategories()
        {
            var categories = await _categoryRepository.GetAll().ToListAsync();
            return categories;
        }
    }
}
