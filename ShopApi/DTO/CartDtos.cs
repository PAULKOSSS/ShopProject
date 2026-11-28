namespace ShopApi.DTO
{
    public class CartDtos
    {
        public record CartItemResponse(int Id, int ProductId, string ProductName, string? ImagePath, int ProductVariantId, string SizeOrPackage, decimal UnitPrice, int Quantity, decimal TotalPrice, int QuantityInStock, bool IsAvailable);
        public record CartResponse(List<CartItemResponse> Items, decimal TotalPrice);
        public record AddToCartRequest(int ProductVariantId, int Quantity);
        public record UpdateCartItemRequest(int Quantity);
    }
}
