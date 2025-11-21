using GameLogBack.DbContext;
using GameLogBack.Dtos;
using GameLogBack.Dtos.Category;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Services;



public class  CategoryService : ICategoryService
{
    private readonly GameLogDbContext _context;

    public CategoryService(GameLogDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CategoryDto>> GetUserCategories(string userId)
    {
        var categories = await _context.Categories.Where(x => x.UserId == userId).Select(x => new CategoryDto()
        {
            CategoryId = x.CategoryId,
            CategoryName = x.CategoryName,
            Description = x.Description,
            CreatedDate = x.CreatedDate,
            UpdatedDate = x.UpdatedDate,
            CreatedBy = x.CreatedBy,
            UpdatedBy = x.UpdatedBy,
        }).ToListAsync();
        
        return categories;
    }

    public async Task<CategoryDto> GetCategory(string categoryId)
    {
        var category = await _context.Categories.Where(x => x.CategoryId == categoryId).Select(x => new CategoryDto()
        {
            CategoryId = x.CategoryId,
            CategoryName = x.CategoryName,
            Description = x.Description,
            CreatedDate = x.CreatedDate,
            UpdatedDate = x.UpdatedDate,
            CreatedBy = x.CreatedBy,
            UpdatedBy = x.UpdatedBy,
        }).FirstOrDefaultAsync();
        return category ?? throw new NotFoundException("Category not fund");
    }

    public async Task<CategoryDto> CreateCategory(CategoryPostDto categoryPostDto, string userId)
    {
        var isCategoryExist = await _context.Categories
            .AnyAsync(x => x.CategoryName == categoryPostDto.CategoryName && x.UserId == userId);
        if (isCategoryExist)
        {
            throw new BadRequestException("Category with this name already exist");
        }

        var newCategory = new Categories()
        {
            CategoryId = Guid.NewGuid().ToString(),
            CategoryName = categoryPostDto.CategoryName,
            Description = categoryPostDto.Description,
            UserId = userId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedBy = userId,
        };
        _context.Categories.Add(newCategory);
        await _context.SaveChangesAsync();
        return new CategoryDto()
        {
            CategoryId = newCategory.CategoryId,
            CategoryName = newCategory.CategoryName,
            Description = newCategory.Description,
            CreatedDate = newCategory.CreatedDate,
            UpdatedDate = newCategory.UpdatedDate,
            CreatedBy = newCategory.CreatedBy,
            UpdatedBy = newCategory.UpdatedBy,
        };
    }

    public async Task<CategoryDto> UpdateCategory(CategoryPutDto categoryPutDto, string categoryId, string userId)
    {
        var category = _context.Categories.FirstOrDefault(x => x.CategoryId == categoryId);
        if (category is null)
        {
            throw new NotFoundException("Category not found");
        }
        var isCategoryNameExist = _context.Categories.Any(x => x.CategoryName == categoryPutDto.CategoryName && x.CategoryId != categoryId);
        if (isCategoryNameExist)
        {
            throw new BadRequestException("Category with this name already exist");       
        }
        category.CategoryName = categoryPutDto.CategoryName;
        category.Description = categoryPutDto.Description;
        category.UpdatedBy = userId;
        category.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return new CategoryDto()
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            CreatedDate = category.CreatedDate,
            UpdatedDate = category.UpdatedDate,
            CreatedBy = category.CreatedBy,
            UpdatedBy = category.UpdatedBy,
        };
    }

    public async Task DeleteCategory(string categoryId)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.CategoryId == categoryId);
        if (category is null)
        {
            throw new BadRequestException("Category not found");
        }
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryByUserIdDto>> GetCategoriesByUserId(string userId)
    {
        var categories = await _context.Categories.Where(x => x.UserId == userId).Select(x => new CategoryByUserIdDto()
        {
            CategoryId = x.CategoryId,
            CategoryName = x.CategoryName,
            Description = x.Description,
            CreatedDate = x.CreatedDate,
            UpdatedDate = x.UpdatedDate,
        }).ToListAsync();
        return categories;
    }
}