using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GameLogBack.DbContext;
using GameLogBack.Dtos.Category;
using GameLogBack.Dtos.Category.RequestDto;
using GameLogBack.Dtos.Category.ResponseDto;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Services;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GameLogBack.Tests.Services;

[TestSubject(typeof(CategoryService))]
public class CategoryServiceTest
{
    private GameLogDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<GameLogDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new GameLogDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task GetUserCategories_ForValidParams_ReturnsListOfCategoriesForUser()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var paginatedQuery = new PaginatedQuery
        {
            PageNumber = 1,
            PageSize = 5
        };
        var categoriesTableMock = new List<Categories>
        {
            new()
            {
                CategoryId = "1",
                CategoryName = "Kategoria1",
                Description = "",
                CreatedDate = new DateTime(2026, 01, 01),
                UpdatedDate = new DateTime(2026, 01, 02),
                CreatedBy = "Jakub",
                UpdatedBy = "Jakub",
                UserId = "1"
            },
            new()
            {
                CategoryId = "2",
                CategoryName = "Kategoria2",
                Description = "Description",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        var paginatedData = new PaginatedResults<CategoryDto>
        {
            Results =
            [
                new CategoryDto
                {
                    CategoryId = "1",
                    CreatedDate = new DateTime(2026, 01, 01),
                    UpdatedDate = new DateTime(2026, 01, 02),
                    CreatedBy = "Jakub",
                    UpdatedBy = "Jakub",
                    GamesCount = 10
                },

                new CategoryDto
                {
                    CategoryId = "2",
                    CreatedDate = new DateTime(2026, 02, 01),
                    UpdatedDate = new DateTime(2026, 02, 02),
                    CreatedBy = "Piotr",
                    UpdatedBy = "Piotr",
                    GamesCount = 5
                }
            ],
            TotalAmount = 2,
            PageNumber = 1,
            PageSize = 5,
            FirstItemIndexList = 1,
            LastItemIndexList = 2,
            AmountPagesList = [1]
        };
        mockUtilsService.Setup(x => x.GetPaginatedData(It.IsAny<IQueryable<CategoryDto>>(), It.IsAny<PaginatedQuery>()))
            .Returns(Task.FromResult(paginatedData));
        await using var context = GetDbContext();
        context.Categories.AddRange(categoriesTableMock);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = await categoryService.GetUserCategories("1", paginatedQuery);

        //Assert
        result.Results.Should().HaveCount(paginatedData.Results.Count);
    }

    [Fact]
    public async Task GetUserCategories_ForEmptyCategories_ReturnsResultWithEmptyListCategories()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var paginatedQuery = new PaginatedQuery
        {
            PageNumber = 1,
            PageSize = 5
        };
        var categoriesTableMock = new List<Categories>();
        var paginatedData = new PaginatedResults<CategoryDto>
        {
            Results = new List<CategoryDto>(),
            TotalAmount = 0,
            PageNumber = 1,
            PageSize = 5,
            FirstItemIndexList = 0,
            LastItemIndexList = 0,
            AmountPagesList = [1]
        };
        mockUtilsService.Setup(x => x.GetPaginatedData(It.IsAny<IQueryable<CategoryDto>>(), It.IsAny<PaginatedQuery>()))
            .Returns(Task.FromResult(paginatedData));
        await using var mockGameLogDbContext = GetDbContext();
        mockGameLogDbContext.Categories.AddRange(categoriesTableMock);
        await mockGameLogDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var categoryService = new CategoryService(mockGameLogDbContext, mockUtilsService.Object);
        var result = await categoryService.GetUserCategories("1", paginatedQuery);

        //Assert
        result.Results.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCategory_ForValidCategoryId_ReturnsCategory()
    {
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var categoriesTableMock = new List<Categories>
        {
            new()
            {
                CategoryId = "1",
                CategoryName = "Kategoria1",
                Description = "",
                CreatedDate = new DateTime(2026, 01, 01),
                UpdatedDate = new DateTime(2026, 01, 02),
                CreatedBy = "Jakub",
                UpdatedBy = "Jakub",
                UserId = "1"
            },
            new()
            {
                CategoryId = "2",
                CategoryName = "Kategoria2",
                Description = "Description",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        context.Categories.AddRange(categoriesTableMock);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = await categoryService.GetCategory("1");

        //Assert
        result.CategoryName.Should().Be("Kategoria1");
    }

    [Fact]
    public async Task GetCategory_ForInvalidCategoryId_ThrowNotFoundException()
    {
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var categoriesTableMock = new List<Categories>
        {
            new()
            {
                CategoryId = "1",
                CategoryName = "Kategoria1",
                Description = "",
                CreatedDate = new DateTime(2026, 01, 01),
                UpdatedDate = new DateTime(2026, 01, 02),
                CreatedBy = "Jakub",
                UpdatedBy = "Jakub",
                UserId = "1"
            },
            new()
            {
                CategoryId = "2",
                CategoryName = "Kategoria2",
                Description = "Description",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        context.Categories.AddRange(categoriesTableMock);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = async () => { await categoryService.GetCategory("3"); };

        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Category not found");
    }

    [Fact]
    public async Task CreateCategory_ForValidCategory_ReturnNewCreatedCategory()
    {
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var newCategory = new CategoryPostDto
        {
            CategoryName = "Category1",
            Description = ""
        };
        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = await categoryService.CreateCategory(newCategory, "1");

        //Assert
        result.CategoryName.Should().Be(newCategory.CategoryName);
    }

    [Fact]
    public async Task CreateCategory_ForInValidCategory_ThrowBadRequestException()
    {
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var newCategory = new CategoryPostDto
        {
            CategoryName = "Kategoria1",
            Description = ""
        };
        var categoryMock = new Categories
        {
            CategoryId = "1",
            CategoryName = "Kategoria1",
            Description = "",
            CreatedDate = new DateTime(2026, 01, 01),
            UpdatedDate = new DateTime(2026, 01, 02),
            CreatedBy = "Jakub",
            UpdatedBy = "Jakub",
            UserId = "1"
        };
        await context.Categories.AddAsync(categoryMock, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = async () => await categoryService.CreateCategory(newCategory, "1");

        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Category with this name already exist");
    }

    [Fact]
    public async Task UpdateCategory_ForValidCategory_ReturnUpdatedCategory()
    {
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var updateCategory = new CategoryPutDto
        {
            CategoryName = "Kategoria3",
            Description = ""
        };
        var categoriesTableMock = new List<Categories>
        {
            new()
            {
                CategoryId = "1",
                CategoryName = "Kategoria1",
                Description = "",
                CreatedDate = new DateTime(2026, 01, 01),
                UpdatedDate = new DateTime(2026, 01, 02),
                CreatedBy = "Jakub",
                UpdatedBy = "Jakub",
                UserId = "1"
            },
            new()
            {
                CategoryId = "2",
                CategoryName = "Kategoria2",
                Description = "Description",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        await context.Categories.AddRangeAsync(categoriesTableMock, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = await categoryService.UpdateCategory(updateCategory, "1", "1");

        //Assert
        result.CategoryName.Should().Be(updateCategory.CategoryName);
    }

    [Fact]
    public async Task UpdateCategory_ForInvalidCategoryId_ThrowNotFoundException()
    {
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var updateCategory = new CategoryPutDto
        {
            CategoryName = "Kategoria3",
            Description = ""
        };
        var categoriesTableMock = new List<Categories>
        {
            new()
            {
                CategoryId = "1",
                CategoryName = "Kategoria1",
                Description = "",
                CreatedDate = new DateTime(2026, 01, 01),
                UpdatedDate = new DateTime(2026, 01, 02),
                CreatedBy = "Jakub",
                UpdatedBy = "Jakub",
                UserId = "1"
            },
            new()
            {
                CategoryId = "2",
                CategoryName = "Kategoria2",
                Description = "Description",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        await context.Categories.AddRangeAsync(categoriesTableMock, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = async () => await categoryService.UpdateCategory(updateCategory, "5", "1");

        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Category not found");
    }

    [Fact]
    public async Task UpdateCategory_ForInvalidCategoryName_ThrowBadRequestException()
    {
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var updateCategory = new CategoryPutDto
        {
            CategoryName = "Kategoria2",
            Description = ""
        };
        var categoriesTableMock = new List<Categories>
        {
            new()
            {
                CategoryId = "1",
                CategoryName = "Kategoria1",
                Description = "",
                CreatedDate = new DateTime(2026, 01, 01),
                UpdatedDate = new DateTime(2026, 01, 02),
                CreatedBy = "Jakub",
                UpdatedBy = "Jakub",
                UserId = "1"
            },
            new()
            {
                CategoryId = "2",
                CategoryName = "Kategoria2",
                Description = "Description",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        await context.Categories.AddRangeAsync(categoriesTableMock, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = async () => await categoryService.UpdateCategory(updateCategory, "1", "1");

        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Category with this name already exist");
    }

    [Fact]
    public async Task DeleteCategory_ForValidCategoryId_ShouldDeleteCategory()
    {
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var categoriesTableMock = new List<Categories>
        {
            new()
            {
                CategoryId = "1",
                CategoryName = "Kategoria1",
                Description = "",
                CreatedDate = new DateTime(2026, 01, 01),
                UpdatedDate = new DateTime(2026, 01, 02),
                CreatedBy = "Jakub",
                UpdatedBy = "Jakub",
                UserId = "1"
            },
            new()
            {
                CategoryId = "2",
                CategoryName = "Kategoria2",
                Description = "Description",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        await context.Categories.AddRangeAsync(categoriesTableMock, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        await categoryService.DeleteCategory("1");
        var isExistCategory =
            await context.Categories.AnyAsync(x => x.CategoryId == "1", TestContext.Current.CancellationToken);

        //Assert
        isExistCategory.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteCategory_ForInvalidCategoryId_ThrowNotFoundException()
    {
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var categoriesTableMock = new List<Categories>
        {
            new()
            {
                CategoryId = "1",
                CategoryName = "Kategoria1",
                Description = "",
                CreatedDate = new DateTime(2026, 01, 01),
                UpdatedDate = new DateTime(2026, 01, 02),
                CreatedBy = "Jakub",
                UpdatedBy = "Jakub",
                UserId = "1"
            },
            new()
            {
                CategoryId = "2",
                CategoryName = "Kategoria2",
                Description = "Description",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        await context.Categories.AddRangeAsync(categoriesTableMock, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = async () => await categoryService.DeleteCategory("3");


        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Category not found");
    }

    [Fact]
    public async Task DeleteCategory_ForCategoryToDeleteExistGames_BadRequestException()
    {
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var categoriesTableMock = new Categories
        {
            CategoryId = "1",
            CategoryName = "Kategoria1",
            Description = "",
            CreatedDate = new DateTime(2026, 01, 01),
            UpdatedDate = new DateTime(2026, 01, 02),
            CreatedBy = "Jakub",
            UpdatedBy = "Jakub",
            UserId = "1"
        };
        var gamesTableMock = new Games
        {
            GameId = "1",
            GameName = "Gra",
            CreatedDate = new DateTime(2026, 01, 01),
            UpdatedDate = new DateTime(2026, 01, 02),
            CreatedBy = "Jakub",
            UpdatedBy = "Jakub",
            UserId = "1",
            YearPlayed = new DateTime(2026, 01, 02),
            CategoryId = "1"
        };
        await context.Categories.AddAsync(categoriesTableMock, TestContext.Current.CancellationToken);
        await context.Games.AddAsync(gamesTableMock, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = async () => await categoryService.DeleteCategory("1");


        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Exist game with this category");
    }

    [Fact]
    public async Task GetCategoriesByUserId_ForValidUserId_ReturnListCategories()
    {
        //Arrange
        //Arrange
        await using var context = GetDbContext();
        var mockUtilsService = new Mock<IUtilsService>();
        var categoriesTableMock = new List<Categories>
        {
            new()
            {
                CategoryId = "1",
                CategoryName = "Kategoria1",
                Description = "",
                CreatedDate = new DateTime(2026, 01, 01),
                UpdatedDate = new DateTime(2026, 01, 02),
                CreatedBy = "Jakub",
                UpdatedBy = "Jakub",
                UserId = "1"
            },
            new()
            {
                CategoryId = "2",
                CategoryName = "Kategoria2",
                Description = "Description",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        await context.Categories.AddRangeAsync(categoriesTableMock, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        //Act
        var categoryService = new CategoryService(context, mockUtilsService.Object);
        var result = await categoryService.GetCategoriesByUserId("1");

        //Assert
        result.Should().HaveCount(2);
    }
}