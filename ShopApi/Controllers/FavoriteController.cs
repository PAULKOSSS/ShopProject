using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("api/favorites")]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly FavoriteService _favoriteService;
        private int UserId => int.Parse((User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub))!);
        public FavoriteController(FavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavoriteItems()
        {
            var favoriteItems = await _favoriteService.GetFavoriteItemsAsync(UserId);
            return Ok(favoriteItems);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToFavorites(int productId)
        {
            var result = await _favoriteService.AddToFavoritesAsync(UserId, productId);
            if (result != null) return BadRequest(new { Error = result });
            return Ok();
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromFavorites(int productId)
        {
            var result = await _favoriteService.RemoveFromFavoritesAsync(UserId, productId);
            if (result != null) return BadRequest(new { Error = result });
            return Ok();
        }

        [HttpPost("{productId}/to-cart")]
        public async Task<IActionResult> MoveToCart([FromBody] DTO.CartDtos.AddToCartRequest request, int productId)
        {
            var result = await _favoriteService.MoveToCartAsync(request, UserId, productId);
            if (result != null) return BadRequest(new { Error = result });
            return Ok();
        }
    }
}
