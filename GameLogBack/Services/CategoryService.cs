using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Dtos.Category;
using GameLogBack.Dtos.Category.RequestDto;
using GameLogBack.Dtos.Category.ResponseDto;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGameRepository _gameRepository;
    private readonly IUtilsService _utilsService;

    public CategoryService(IUtilsService utilsService, ICategoryRepository categoryRepository, IGameRepository gameRepository)
    {
        _utilsService = utilsService;
        _categoryRepository = categoryRepository;
        _gameRepository = gameRepository;
    }

    public async Task<PaginatedResults<CategoryDto>> GetUserCategories(string userId, PaginatedQuery paginatedQuery)
    {
        var categories = await _categoryRepository.GetByUserId(userId, paginatedQuery);
        var categoriesDtoPaginated = new PaginatedResults<CategoryDto>
        {
            Results = categories.Results.Select(x => new CategoryDto
            {
                CategoryId = x.CategoryId,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                CreatedBy = x.CreatedBy,
                UpdatedBy = x.UpdatedBy,
                CategoryName = x.CategoryName,
                Description = x.Description,
                GamesCount = x.Games.Count
            }).ToList(),
            TotalAmount = categories.TotalAmount,
            PageNumber = categories.PageNumber,
            PageSize = categories.PageSize,
            FirstItemIndexList = categories.FirstItemIndexList,
            LastItemIndexList = categories.LastItemIndexList
        };
        return categoriesDtoPaginated;
    }

    public async Task<CategoryDto> GetCategory(string categoryId, string userId)
    {
        var category = await _categoryRepository.GetById(categoryId, userId);
        if (category is null) throw new NotFoundException("Category not found");
        var categoryWithGamesCounter = new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            CreatedDate = category.CreatedDate,
            UpdatedDate = category.UpdatedDate,
            CreatedBy = category.CreatedBy,
            UpdatedBy = category.UpdatedBy,
            GamesCount = category.Games.Count
        };
        return categoryWithGamesCounter;
    }

    public async Task<CategoryDto> CreateCategory(CategoryPostDto categoryPostDto, string userId)
    {
        var isCategoryExist = await _categoryRepository.CheckIfExists(categoryPostDto.CategoryName, userId);
        if (isCategoryExist) throw new BadRequestException("Category with this name already exist");
        var newCategory = new Categories
        {
            CategoryId = Guid.NewGuid().ToString(),
            CategoryName = categoryPostDto.CategoryName,
            Description = categoryPostDto.Description,
            UserId = userId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedBy = userId
        };
        await _categoryRepository.Create(newCategory);
        return new CategoryDto
        {
            CategoryId = newCategory.CategoryId,
            CategoryName = newCategory.CategoryName,
            Description = newCategory.Description,
            CreatedDate = newCategory.CreatedDate,
            UpdatedDate = newCategory.UpdatedDate,
            CreatedBy = newCategory.CreatedBy,
            UpdatedBy = newCategory.UpdatedBy
        };
    }

    public async Task<CategoryDto> UpdateCategory(CategoryPutDto categoryPutDto, string categoryId, string userId)
    {
        var category = await _categoryRepository.GetById(categoryId, userId);
        if (category is null) throw new NotFoundException("Category not found");
        var isCategoryNameExist = await _categoryRepository.CheckIfExistsWithSameName(categoryPutDto.CategoryName, userId, categoryId);
        if (isCategoryNameExist) throw new BadRequestException("Category with this name already exist");
        category.CategoryName = categoryPutDto.CategoryName;
        category.Description = categoryPutDto.Description;
        category.UpdatedBy = userId;
        category.UpdatedDate = DateTime.UtcNow;
        await _categoryRepository.Update(category);
        return new CategoryDto
        {
            CategoryId = category.CategoryId,
            CategoryName = category.CategoryName,
            Description = category.Description,
            CreatedDate = category.CreatedDate,
            UpdatedDate = category.UpdatedDate,
            CreatedBy = category.CreatedBy,
            UpdatedBy = category.UpdatedBy
        };
    }

    public async Task DeleteCategory(string categoryId, string userId)
    {
        var category = await _categoryRepository.GetById(categoryId, userId);
        if (category is null) throw new NotFoundException("Category not found");
        var isGameWithCategoryExist = await _gameRepository.CheckIfGameExitsById(categoryId);
        if (isGameWithCategoryExist) throw new BadRequestException("Exist game with this category");
        await _categoryRepository.Delete(category);
    }

    public async Task<PaginatedResults<CategoryByUserIdDto>> GetCategoriesByUserId(string userId, PaginatedQuery paginatedQuery)
    {
        var categories = await _categoryRepository.GetByUserId(userId, paginatedQuery);
        var categoriesByUserIdDtoPaginated = new PaginatedResults<CategoryByUserIdDto>
        {
            Results = categories.Results.Select(x => new CategoryByUserIdDto()
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                Description = x.Description,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                GamesCount = x.Games.Count
            }).ToList(),
            TotalAmount = categories.TotalAmount,
            PageNumber = categories.PageNumber,
            PageSize = categories.PageSize,
            FirstItemIndexList = categories.FirstItemIndexList,
            LastItemIndexList = categories.LastItemIndexList
        };
        return categoriesByUserIdDtoPaginated;
    }
}