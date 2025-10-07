using GameLogBack.DbContext;
using GameLogBack.Dtos;
using GameLogBack.Dtos.Category;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;

namespace GameLogBack.Services;



public class CategoryService : ICategoryService
{
    private readonly GameLogDbContext _context;

    public CategoryService(GameLogDbContext context)
    {
        _context = context;
    }

    public IEnumerable<CategoryDto> GetUserCategories(string userId)
    {
        var categories = _context.Categories.Where(x => x.UserId == userId).Select(x => new CategoryDto()
        {
            CategoryId = x.CategoryId,
            CategoryName = x.CategoryName,
            Description = x.Description,
            CreatedDate = x.CreatedDate,
            UpdatedDate = x.UpdatedDate,
            CreatedBy = x.CreatedBy,
            UpdatedBy = x.UpdatedBy,
        }).ToList();
        
        return categories;
    }

    public CategoryDto GetCategory(string categoryId)
    {
        var category = _context.Categories.Where(x => x.CategoryId == categoryId).Select(x => new CategoryDto()
        {
            CategoryId = x.CategoryId,
            CategoryName = x.CategoryName,
            Description = x.Description,
            CreatedDate = x.CreatedDate,
            UpdatedDate = x.UpdatedDate,
            CreatedBy = x.CreatedBy,
            UpdatedBy = x.UpdatedBy,
        }).FirstOrDefault();
        if (category is null)
        {
            throw new BadRequestException("Category not found");
        }
        return category;
    }

    public void CreateCategory(CategoryPostDto categoryPostDto, string userId)
    {
        var isCategoryExist = _context.Categories.Any(x => x.CategoryName == categoryPostDto.CategoryName);
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
        _context.SaveChanges();
    }

    public void UpdateCategory(CategoryPutDto categoryPutDto, string categoryId, string userId)
    {
        var category = _context.Categories.FirstOrDefault(x => x.CategoryId == categoryId);
        if (category is null)
        {
            throw new BadRequestException("Category not found");
        }
        category.CategoryName = categoryPutDto.CategoryName;
        category.Description = categoryPutDto.Description;
        category.UpdatedBy = userId;
        category.UpdatedDate = DateTime.UtcNow;
        _context.SaveChanges();
    }

    public void DeleteCategory(string categoryId)
    {
        var category = _context.Categories.FirstOrDefault(x => x.CategoryId == categoryId);
        if (category is null)
        {
            throw new BadRequestException("Category not found");
        }
        _context.Categories.Remove(category);
        _context.SaveChanges();
    }
}