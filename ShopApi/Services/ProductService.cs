using ShopApi.Models;
using ShopApi.DTO;
using Microsoft.EntityFrameworkCore;

namespace ShopApi.Services
{
    public class ProductService
    {
        private readonly ISRPO_PR4Context _context;
        public ProductService(ISRPO_PR4Context context)
        {
            _context = context;
        }

        public async Task<List<ProductDtos.ProductResponse>> GetAllProductsAsync(string? search, int? categoryId, decimal? minPrice, decimal? maxPrice, bool? isOnlyAvailable)
        {
            var query = _context.Products.AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            if (isOnlyAvailable == true)
            {
                query = query.Where(p => p.IsAvailable);
            }

            return await query
            .Select(p => new ProductDtos.ProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.CategoryId,
                p.Category.Name,
                p.ImagePath,
                p.IsAvailable,
                p.AverageRating,
                p.ProductVariants.Select(v => new ProductDtos.ProductVariantResponse(
                    v.Id,
                    v.SizeOrPackage,
                    v.QuantityInStock
                )).ToList()
            ))
            .ToListAsync();
        }

        public async Task<ProductDtos.ProductResponse?> GetProductByIdAsync(int id)
        {
            return await _context.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDtos.ProductResponse(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.CategoryId,
                p.Category.Name,
                p.ImagePath,
                p.IsAvailable,
                p.AverageRating,
                p.ProductVariants.Select(v => new ProductDtos.ProductVariantResponse(
                    v.Id,
                    v.SizeOrPackage,
                    v.QuantityInStock
                )).ToList()
            ))
            .FirstOrDefaultAsync();
        }

        public async Task<(ProductDtos.ProductResponse?, string? Error)> CreateProductAsync(ProductDtos.CreateProductRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) return (null, "Имя продукта обязательно");
            if (request.Price < 0) return (null, "Цена продукта не может быть отрицательной");
            if (!await _context.Categories.AnyAsync(c => c.Id == request.CategoryId)) return (null, "Категория не найдена");

            List<ProductVariants> variants = [];

            foreach (var variantRequest in request.ProductVariants)
            {
                if (string.IsNullOrWhiteSpace(variantRequest.SizeOrPackage)) return (null, "Размер или упаковка варианта продукта обязателен");
                if (variantRequest.QuantityInStock < 0) return (null, "Количество варианта продукта не может быть отрицательным");

                var variant = new ProductVariants
                {
                    SizeOrPackage = variantRequest.SizeOrPackage,
                    QuantityInStock = variantRequest.QuantityInStock
                };
                variants.Add(variant);
            }

            var product = new Products
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                ImagePath = request.ImagePath,
                IsAvailable = request.IsAvailable,
                AverageRating = 0,
                ProductVariants = variants
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return (await GetProductByIdAsync(product.Id), null);
        }

        public async Task<(ProductDtos.ProductResponse?, string? Error)> UpdateProductAsync(int id, ProductDtos.UpdateProductRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return (null, "Продукт не найден");

            if (string.IsNullOrWhiteSpace(request.Name)) return (null, "Имя продукта обязательно");
            if (request.Price < 0) return (null, "Цена продукта не может быть отрицательной");
            if (!await _context.Categories.AnyAsync(c => c.Id == request.CategoryId)) return (null, "Категория не найдена");

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.CategoryId = request.CategoryId;
            product.ImagePath = request.ImagePath;
            await _context.SaveChangesAsync();
            return (await GetProductByIdAsync(product.Id), null);
        }

        public async Task<(ProductDtos.ProductResponse?, string? Error)> UpdateAvailabilityAsync(int id, ProductDtos.UpdateAvailabilityRequest request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return (null, "Продукт не найден");

            product.IsAvailable = request.IsAvailable;
            await _context.SaveChangesAsync();
            return (await GetProductByIdAsync(product.Id), null);
        }

        public async Task<(ProductDtos.ProductResponse?, string? Error)> UpdateStockAsync(int variantId, ProductDtos.UpdateStockRequest request)
        {
            var variant = await _context.ProductVariants.FirstOrDefaultAsync(v => v.Id == variantId);
            if (variant == null) return (null, "Вариант продукта не найден");
            if (request.QuantityInStock < 0) return (null, "Количество на складе не может быть отрицательным");

            variant.QuantityInStock = request.QuantityInStock;
            await _context.SaveChangesAsync();
            return (await GetProductByIdAsync(variant.ProductId), null);
        }

        public async Task<(ProductDtos.ProductVariantResponse?, string? Error)> CreateProductVariantAsync(int productId, ProductDtos.CreateProductVariantRequest request)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null) return (null, "Продукт не найден");

            if (string.IsNullOrWhiteSpace(request.SizeOrPackage)) return (null, "Размер или упаковка обязателен");
            if (request.QuantityInStock < 0) return (null, "Количество на складе не может быть отрицательным");

            var variant = new ProductVariants
            {
                ProductId = productId,
                SizeOrPackage = request.SizeOrPackage,
                QuantityInStock = request.QuantityInStock
            };
            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();
            return (new ProductDtos.ProductVariantResponse(variant.Id, variant.SizeOrPackage, variant.QuantityInStock), null);
        }
    }
}
