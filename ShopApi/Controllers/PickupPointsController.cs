using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("api/pickup-points")]
    [Authorize(Roles = "Admin")]
    public class PickupPointsController : ControllerBase
    {
        private readonly PickupPointService _pickupPointService;
        public PickupPointsController(PickupPointService pickupPointService)
        {
            _pickupPointService = pickupPointService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPickupPoints()
        {
            var pickupPoints = await _pickupPointService.GetAllPickupPointsAsync();
            return Ok(pickupPoints);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePickupPoint([FromBody] DTO.PickupPointDtos.PickupPointRequest request)
        {
            var (pickupPoint, error) = await _pickupPointService.CreateAsync(request);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(pickupPoint);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePickupPoint(int id, [FromBody] DTO.PickupPointDtos.PickupPointRequest request)
        {
            var (pickupPoint, error) = await _pickupPointService.UpdateAsync(id, request);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(pickupPoint);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePickupPoint(int id)
        {
            var (pickupPoint, error) = await _pickupPointService.DeleteAsync(id);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(pickupPoint);
        }
    }
}
