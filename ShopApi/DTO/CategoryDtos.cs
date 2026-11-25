namespace ShopApi.DTO
{
    public class CategoryDtos
    {
        public record CategoryResponse(int Id, string Name);
        public record CategoryRequest(string Name);
    }
}
