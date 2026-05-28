using System.Security.Claims;
using FluentValidation;
using GameLogBack.Dtos.Category;
using GameLogBack.Dtos.Category.RequestDto;
using GameLogBack.Dtos.Category.ResponseDto;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Interfaces;
using GameLogBack.Validators;
using GameLogBack.Validators.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameLogBack.Controllers;

[Route("api/category")]
[ApiController]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IValidator<CategoryPostDto> _categoryPostDtoValidator;
    private readonly IValidator<CategoryPutDto> _categoryPutDtoValidator;

    public CategoryController(ICategoryService categoryService, IValidator<CategoryPostDto> categoryPostDtoValidator, IValidator<CategoryPutDto> categoryPutDtoValidator)
    {
        _categoryService = categoryService;
        _categoryPostDtoValidator = categoryPostDtoValidator;
        _categoryPutDtoValidator = categoryPutDtoValidator;
    }

    [HttpGet("get-user-categories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetUserCategories(
        [FromQuery] PaginatedQuery paginatedQuery)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var categories = await _categoryService.GetUserCategories(userId, paginatedQuery);
        return Ok(categories);
    }

    [HttpGet("get-categories-by-userId/{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategoriesByUserId([FromRoute] string userId)
    {
        var categories = await _categoryService.GetCategoriesByUserId(userId);
        return Ok(categories);
    }

    [HttpGet("get-category/{categoryId}")]
    public async Task<ActionResult<CategoryDto>> GetCategory([FromRoute] string categoryId)
    {
        var category = await _categoryService.GetCategory(categoryId);
        return Ok(category);
    }

    [HttpPost("create-category")]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryPostDto newCategory)
    {
        var result = await _categoryPostDtoValidator.ValidateAsync(newCategory);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e =>  new {e.PropertyName, Errors = new List<object>(){e.ErrorMessage}});
            return BadRequest(errors);
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var category = await _categoryService.CreateCategory(newCategory, userId);
        return Ok(category);
    }

    [HttpPut("update/{categoryId}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory([FromBody] CategoryPutDto categoryPutDto,
        [FromRoute] string categoryId)
    {
        var result = await _categoryPutDtoValidator.ValidateAsync(categoryPutDto);

        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e =>  new {e.PropertyName, Errors = new List<object>(){e.ErrorMessage}});
            return BadRequest(errors);
        }
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
