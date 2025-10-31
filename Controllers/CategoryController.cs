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
        public ActionResult<IEnumerable<CategoryDto>> GetUserCategories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var categories = _categoryService.GetUserCategories(userId);
            return Ok(categories);
        }

        [HttpGet("get-category/{categoryId}")]
        [Authorize]
        public ActionResult<CategoryDto> GetCategory([FromRoute]string categoryId)
        {
            var category = _categoryService.GetCategory(categoryId);
            return Ok(category);       
        }

        [HttpPost("create-category")]
        [Authorize]
        public ActionResult<CategoryDto> CreateCategory([FromBody] CategoryPostDto newCategory)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var category = _categoryService.CreateCategory(newCategory, userId);
            return Ok(category);
        }
        
        [HttpPut("update/{categoryId}")]

        public ActionResult<CategoryDto> UpdateCategory([FromBody] CategoryPutDto categoryPutDto, [FromRoute] string categoryId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var category = _categoryService.UpdateCategory(categoryPutDto, categoryId, userId);
            return Ok(category);
        }

        [HttpDelete("delete/{categoryId}")]
        public ActionResult DeleteCategory([FromRoute] string categoryId)
        {
            _categoryService.DeleteCategory(categoryId);
            return Ok();       
        }
    }
}
