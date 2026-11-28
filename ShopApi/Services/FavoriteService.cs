using Microsoft.EntityFrameworkCore;
using ShopApi.DTO;
using ShopApi.Models;

namespace ShopApi.Services
{
    public class FavoriteService
    {
        private readonly ISRPO_PR4Context _context;
        private readonly CartService _cartService;
        public FavoriteService(ISRPO_PR4Context context)
        {
            _context = context;
            _cartService = new CartService(_context);
        }

        public async Task<List<ProductDtos.ProductResponse>> GetFavoriteItemsAsync(int userId)
        {
            var favoriteItems = await _context.Favorites.Where(fi => fi.UserId == userId).Select(fi => new ProductDtos.ProductResponse(
                    fi.Product.Id,
                    fi.Product.Name,
                    fi.Product.Description,
                    fi.Product.Price,
                    fi.Product.CategoryId,
                    fi.Product.Category.Name,
                    fi.Product.ImagePath,
                    fi.Product.IsAvailable,
                    fi.Product.AverageRating,
                    fi.Product.ProductVariants.Select(pv => new ProductDtos.ProductVariantResponse(pv.Id,pv.SizeOrPackage,pv.QuantityInStock)).ToList())).ToListAsync();
            return favoriteItems;
        }

        public async Task<string?> AddToFavoritesAsync(int userId, int productId)
        {
            if (!await _context.Products.AnyAsync(p => p.Id == productId)) return "Продукт не найден.";

            var existingFavorite = await _context.Favorites.AnyAsync(f => f.UserId == userId && f.ProductId == productId);
            if (existingFavorite) return "Продукт уже в избранном.";

            var favorite = new Favorites
            {
                UserId = userId,
                ProductId = productId
            };
            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<string?> RemoveFromFavoritesAsync(int userId, int productId)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
            if (favorite == null) return "Продукт не в избранном.";

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<string?> MoveToCartAsync(CartDtos.AddToCartRequest request, int userId, int productId)
        {
            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
            if (favorite == null) return "Продукт не в избранном.";

            var check = await _context.ProductVariants.AnyAsync(v => v.Id == request.ProductVariantId && v.ProductId == productId);
            if (!check) return "Вариант продукта не принадлежит указанному продукту.";

            await _cartService.AddToCartAsync(userId, request);
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return null;
        }
    }
}
