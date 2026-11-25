namespace ShopApi.DTO
{
    public class PickupPointDtos
    {
        public record PickupPointResponse(int Id, string Address, string? Description);
        public record PickupPointRequest(string Address, string? Description);
    }
}
