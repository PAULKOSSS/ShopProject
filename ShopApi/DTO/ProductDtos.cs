namespace ShopApi.DTO
{
    public class ProductDtos
    {
        public record ProductVariantResponse(int Id, string SizeOrPackage, int QuantityInStock);
        public record ProductResponse(int Id, string Name, string? Description, decimal Price, int CategoryId, string CategoryName, string? ImagePath, bool IsAvailable, decimal AverageRating, List<ProductVariantResponse> ProductVariants);
        public record CreateProductVariantRequest(string SizeOrPackage, int QuantityInStock);
        public record CreateProductRequest(string Name, string? Description, decimal Price, int CategoryId, string? ImagePath, bool IsAvailable, List<CreateProductVariantRequest> ProductVariants);
        public record UpdateProductRequest(string Name, string? Description, decimal Price, int CategoryId, string? ImagePath);
        public record UpdateAvailabilityRequest(bool IsAvailable);
        public record UpdateStockRequest(int QuantityInStock);
    }
}
