using GameLogBack.Dtos.Category;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;

namespace GameLogBack.Interfaces;

public interface ICategoryService
{
    Task<PaginatedResults<CategoryDto>> GetUserCategories(string userId, PaginatedQuery paginatedQuery);
    Task<CategoryDto> GetCategory(string categoryId);
    Task<CategoryDto> CreateCategory(CategoryPostDto categoryPostDto, string userId);
    Task<CategoryDto> UpdateCategory(CategoryPutDto categoryPutDto, string categoryId, string userId);
    Task DeleteCategory(string categoryId);
    Task<IEnumerable<CategoryByUserIdDto>> GetCategoriesByUserId(string userId);
}