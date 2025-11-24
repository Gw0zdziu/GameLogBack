using System.Security.Claims;
using GameLogBack.Dtos;
using GameLogBack.Dtos.Category;
using GameLogBack.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-user-categories")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetUserCategories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var categories = await _categoryService.GetUserCategories(userId);
            return Ok(categories);
        }

        [HttpGet("get-categories-by-userId/{userId}")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesByUserId([FromRoute] string userId)
        {
            var categories = await _categoryService.GetCategoriesByUserId(userId);
            return Ok(categories);
        }

        [HttpGet("get-category/{categoryId}")]
        [Authorize]
        public async Task<ActionResult<CategoryDto>> GetCategory([FromRoute]string categoryId)
        {
            var category = await _categoryService.GetCategory(categoryId);
            return Ok(category);       
        }

        [HttpPost("create-category")]
        [Authorize]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryPostDto newCategory)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var category = await _categoryService.CreateCategory(newCategory, userId);
            return Ok(category);
        }
        
        [HttpPut("update/{categoryId}")]

        public async Task<ActionResult<CategoryDto>> UpdateCategory([FromBody] CategoryPutDto categoryPutDto, [FromRoute] string categoryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var category = await _categoryService.UpdateCategory(categoryPutDto, categoryId, userId);
            return Ok(category);
        }

        [HttpDelete("delete/{categoryId}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] string categoryId)
        {
            await _categoryService.DeleteCategory(categoryId);
            return Ok();       
        }
    }
}
