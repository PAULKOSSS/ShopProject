using Microsoft.AspNetCore.Mvc;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] DTO.AuthDtos.RegisterRequest request)
        {
            var (data, error) = await _authService.RegisterAsync(request);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(data);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] DTO.AuthDtos.LoginRequest request)
        {
            var data = await _authService.LoginAsync(request);
            if (data == null) return Unauthorized(new { Error = "Неверный логин или пароль." });
            return Ok(data);
        }
    }
}
