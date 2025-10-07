using GameLogBack.Dtos;
using GameLogBack.Dtos.Category;

namespace GameLogBack.Interfaces;

public interface ICategoryService
{
    IEnumerable<CategoryDto> GetUserCategories(string userId);
    CategoryDto GetCategory(string categoryId);
    void CreateCategory(CategoryPostDto categoryPostDto, string userId);
    void UpdateCategory(CategoryPutDto categoryPutDto, string categoryId, string userId);
    void DeleteCategory(string categoryId);
}