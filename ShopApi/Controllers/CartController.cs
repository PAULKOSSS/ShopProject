using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;
        private int UserId => int.Parse((User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub))!);
        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var cartResponse = await _cartService.GetCartItemsAsync(UserId);
            return Ok(cartResponse);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] DTO.CartDtos.AddToCartRequest request)
        {
            var result = await _cartService.AddToCartAsync(UserId, request);
            if (result != null) return BadRequest(new { Error = result });
            return Ok();
        }

        [HttpPut("{cartItemId}")]
        public async Task<IActionResult> UpdateCartItem(int cartItemId, [FromBody] DTO.CartDtos.UpdateCartItemRequest request)
        {
            var result = await _cartService.UpdateCartItemAsync(UserId, cartItemId, request);
            if (result != null) return BadRequest(new { Error = result });
            return Ok();
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int cartItemId)
        {
            var result = await _cartService.RemoveCartItemAsync(UserId, cartItemId);
            if (result != null) return BadRequest(new { Error = result });
            return Ok();
        }
        
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            await _cartService.ClearCartAsync(UserId);
            return Ok();
        }

        private int GetUserId()
        {
            var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return int.Parse(idClaim!);
        }
    }
}
