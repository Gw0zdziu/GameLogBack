using GameLogBack.DbContext;
using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Repositories;

public class CategoryRepository: ICategoryRepository
{
    private readonly DbSet<Categories> _categories;
    private readonly GameLogDbContext _context;

    public CategoryRepository(GameLogDbContext context)
    {
        _context = context;
        _categories = context.Categories;
    }


    public  IQueryable<Categories> GetByUserId(string id)
    {
        return _categories.Where(x => x.UserId == id);
    }

    public  IQueryable<Categories> GetById(string id)
    {
        return  _categories.Where(x => x.CategoryId == id);
    }

    public async Task Create(Categories category)
    {
        _categories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Categories category)
    {
        _categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public Task Delete(Categories category)
    {
        _categories.Remove(category);
        return _context.SaveChangesAsync();
    }
}