using Microsoft.EntityFrameworkCore;
using ShopApi.Models;
using ShopApi.DTO;

namespace ShopApi.Services
{
    public class CategoryService
    {
        private readonly ISRPO_PR4Context _context;

        public CategoryService(ISRPO_PR4Context context)
        {
            _context = context;
        }

        public async Task<List<CategoryDtos.CategoryResponse>> GetAllCategoriesAsync()
        {
            return await _context.Categories.OrderBy(c => c.Name).Select(c => new CategoryDtos.CategoryResponse(c.Id, c.Name)).ToListAsync();
        }

        public async Task<(CategoryDtos.CategoryResponse?, string? Error)> CreateAsync(CategoryDtos.CategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) return (null, "Имя категории обязательно");

            var exists = await _context.Categories.AnyAsync(c => c.Name == request.Name);
            if (exists) return (null, "Категория с таким именем уже существует");

            var category = new Categories
            {
                Name = request.Name
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return (new CategoryDtos.CategoryResponse(category.Id, category.Name), null);
        }

        public async Task<(CategoryDtos.CategoryResponse?, string? Error)> UpdateAsync(int id, CategoryDtos.CategoryRequest request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return (null, "Категория не найдена");

            if (string.IsNullOrWhiteSpace(request.Name)) return (null, "Имя категории обязательно");

            var exists = await _context.Categories.AnyAsync(c => c.Name == request.Name && c.Id != id);
            if (exists) return (null, "Категория с таким именем уже существует");

            category.Name = request.Name;
            await _context.SaveChangesAsync();
            return (new CategoryDtos.CategoryResponse(category.Id, category.Name), null);
        }
    }
}
