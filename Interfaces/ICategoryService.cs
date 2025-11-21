using GameLogBack.Dtos;
using GameLogBack.Dtos.Category;

namespace GameLogBack.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetUserCategories(string userId);
    Task<CategoryDto> GetCategory(string categoryId);
    Task<CategoryDto> CreateCategory(CategoryPostDto categoryPostDto, string userId);
    Task<CategoryDto> UpdateCategory(CategoryPutDto categoryPutDto, string categoryId, string userId);
    Task DeleteCategory(string categoryId);
    Task<IEnumerable<CategoryByUserIdDto>> GetCategoriesByUserId(string userId);
}