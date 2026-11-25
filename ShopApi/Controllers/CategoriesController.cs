using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopApi.Services;

namespace ShopApi.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;
        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] DTO.CategoryDtos.CategoryRequest request)
        {
            var (category, error) = await _categoryService.CreateAsync(request);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(category);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] DTO.CategoryDtos.CategoryRequest request)
        {
            var (category, error) = await _categoryService.UpdateAsync(id, request);
            if (error != null) return BadRequest(new { Error = error });
            return Ok(category);
        }
    }
}
