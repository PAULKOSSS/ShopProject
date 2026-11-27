using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.DTO;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("api/products")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase
    {
        private readonly Services.ProductService _productService;
        public ProductsController(Services.ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProducts([FromQuery] string? search, [FromQuery] int? categoryId, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] bool? isOnlyAvailable)
        {
            var products = await _productService.GetAllProductsAsync(search, categoryId, minPrice, maxPrice, isOnlyAvailable);
            return Ok(products);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDtos.CreateProductRequest request)
        {
            var (product, error) = await _productService.CreateProductAsync(request);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDtos.UpdateProductRequest request)
        {
            var (product, error) = await _productService.UpdateProductAsync(id, request);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(product);
        }

        [HttpPatch("{id}/availability")]
        public async Task<IActionResult> UpdateProductAvailability(int id, [FromBody] ProductDtos.UpdateAvailabilityRequest request)
        {
            var (product, error) = await _productService.UpdateAvailabilityAsync(id, request);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(product);
        }

        [HttpPatch("variants/{variantId}/stock")]
        public async Task<IActionResult> UpdateProductStock(int variantId, [FromBody] ProductDtos.UpdateStockRequest request)
        {
            var (product, error) = await _productService.UpdateStockAsync(variantId, request);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(product);
        }

        [HttpPost("{id}/variants")]
        public async Task<IActionResult> CreateProductVariant(int id, [FromBody] ProductDtos.CreateProductVariantRequest request)
        {
            var (variant, error) = await _productService.CreateProductVariantAsync(id, request);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(variant);
        }
    }
}
