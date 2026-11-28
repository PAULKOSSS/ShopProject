using ShopApi.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using ShopApi.DTO;

namespace ShopApi.Services
{
    public class CartService
    {
        private readonly ISRPO_PR4Context _context;
        public CartService(ISRPO_PR4Context context)
        {
            _context = context;
        }

        public async Task<CartDtos.CartResponse> GetCartItemsAsync(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Select(ci => new CartDtos.CartItemResponse(
                    ci.Id,
                    ci.ProductVariant.ProductId,
                    ci.ProductVariant.Product.Name,
                    ci.ProductVariant.Product.ImagePath,
                    ci.ProductVariantId,
                    ci.ProductVariant.SizeOrPackage,
                    ci.ProductVariant.Product.Price,
                    ci.Quantity,
                    ci.Quantity * ci.ProductVariant.Product.Price,
                    ci.ProductVariant.QuantityInStock,
                    ci.ProductVariant.Product.IsAvailable
                ))
                .ToListAsync();
            var totalPrice = cartItems.Sum(ci => ci.TotalPrice);
            return new CartDtos.CartResponse(cartItems, totalPrice);
        }

        public async Task<string?> AddToCartAsync(int userId, CartDtos.AddToCartRequest request)
        {
            if (request.Quantity <= 0) return "Количество должно быть больше нуля.";

            var newVariant = await _context.ProductVariants.Include(pv => pv.Product).FirstOrDefaultAsync(pv => pv.Id == request.ProductVariantId);
            if (newVariant == null || !newVariant.Product.IsAvailable || newVariant.QuantityInStock == 0) return "Вариант продукта не найден или недоступен.";

            var existingCartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductVariantId == request.ProductVariantId);
            if (existingCartItem != null)
            {
                int newQuantity = existingCartItem.Quantity + request.Quantity;
                if (newQuantity > newVariant.QuantityInStock) return "Недостаточно товара на складе.";
                existingCartItem.Quantity = newQuantity;
            }
            else
            {
                if (request.Quantity > newVariant.QuantityInStock) return "Недостаточно товара на складе.";
                var cartItem = new CartItems
                {
                    UserId = userId,
                    ProductVariantId = request.ProductVariantId,
                    Quantity = request.Quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<string?> UpdateCartItemAsync(int userId, int cartItemId, CartDtos.UpdateCartItemRequest request)
        {
            if (request.Quantity <= 0) return "Количество должно быть больше нуля.";

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.UserId == userId);
            if (cartItem == null) return "Товар в корзине не найден.";

            var productVariant = await _context.ProductVariants.Include(pv => pv.Product).FirstOrDefaultAsync(pv => pv.Id == cartItem.ProductVariantId);
            if (productVariant == null || !productVariant.Product.IsAvailable || productVariant.QuantityInStock == 0) return "Вариант продукта не найден или недоступен.";
            if (request.Quantity > productVariant.QuantityInStock) return "Недостаточно товара на складе.";

            cartItem.Quantity = request.Quantity;
            await _context.SaveChangesAsync();
            return null;
        }

        public async Task<string?> RemoveCartItemAsync(int userId, int cartItemId)
        {
            var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.UserId == userId);
            if (cartItem == null) return "Товар в корзине не найден.";

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return null;
        }

        public async Task ClearCartAsync(int userId)
        {
            var cartItems = await _context.CartItems.Where(ci => ci.UserId == userId).ToListAsync();
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
        }
    }
}
