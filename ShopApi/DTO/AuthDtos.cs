namespace ShopApi.DTO
{
    public class AuthDtos
    {
        public record RegisterRequest(string Login, string Password, string SecPassword, string PhoneNumber, string? DeliveryAddress);
        public record LoginRequest(string Login, string Password);
        public record AuthResponse(string Token, string Login, string Role);
    }
}
