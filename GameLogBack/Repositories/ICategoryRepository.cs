using GameLogBack.Entities;

namespace GameLogBack.Repositories;

public interface ICategoryRepository
{
    public  IQueryable<Categories>  GetByUserId(string id);
    public IQueryable<Categories> GetById(string id);
    public Task Create(Categories category);
    public Task Update(Categories category);
    public Task Delete(Categories category);
}