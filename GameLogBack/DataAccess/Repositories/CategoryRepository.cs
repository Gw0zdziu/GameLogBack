using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using GameLogBack.Entities;
using GameLogBack.Extensions;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.DataAccess.Repositories;

public class CategoryRepository: ICategoryRepository
{
    private readonly DbSet<Categories> _categories;
    private readonly GameLogDbContext _context;

    public CategoryRepository(GameLogDbContext context)
    {
        _context = context;
        _categories = context.Categories;
    }


    public async Task<PaginatedResults<Categories>> GetByUserId(string id, PaginatedQuery paginatedQuery)
    {
        return await _categories.Where(x => x.UserId == id).Include(x => x.Games).GetPaginatedData(paginatedQuery);
    }
    
    public async  Task<Categories> GetById(string id)
    {
        return await _categories.Where(x => x.CategoryId == id).Include(x => x.Games).FirstOrDefaultAsync();
    }

    public async Task<string> GetCategoryName(string id)
    {
        return await _categories.Where(x => x.CategoryId == id).Select(x => x.CategoryName).FirstOrDefaultAsync();
    }

    public async Task<bool> CheckIfExists(string categoryName, string userId)
    {
        return await _context.Categories
            .AnyAsync(x => x.CategoryName == categoryName && x.UserId == userId);
    }

    public async Task<bool> CheckIfExistsWithSameName(string categoryName, string userId, string categoryId)
    {
        return await _context.Categories.AnyAsync(x =>
            x.CategoryId != categoryId && x.UserId == userId && x.CategoryName.ToLower() == categoryName.ToLower());
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
