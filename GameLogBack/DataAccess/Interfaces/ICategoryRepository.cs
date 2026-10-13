using GameLogBack.Dtos.Category.ResponseDto;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using GameLogBack.Entities;

namespace GameLogBack.DataAccess.Interfaces;

public interface ICategoryRepository
{
    public  Task<PaginatedResults<Categories>>  GetByUserId(string id, PaginatedQuery paginatedQuery);
    public Task<List<CategoriesNameDto>> GetCategoryNames(string userId);
    public Task<Categories> GetById(string id, string userId);
    public Task<string> GetCategoryName(string id);
    public Task<bool> CheckIfExists(string categoryName, string userId);
    public Task<bool> CheckIfExistsWithSameName(string categoryName, string userId, string categoryId);
    public Task Create(Categories category);
    public Task Update(Categories category);
    public Task Delete(Categories category);
}
