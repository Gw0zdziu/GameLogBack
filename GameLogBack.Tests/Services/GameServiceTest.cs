using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GameLogBack.DbContext;
using GameLogBack.Dtos.Game;
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

[TestSubject(typeof(GameService))]
public class GameServiceTest
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
    public async Task GetGames_ForValidData_ReturnsCorrectData()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gamesMock = new List<Games>
        {
            new()
            {
                GameId = "1",
                GameName = "Battlefield",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            },
            new()
            {
                GameId = "2",
                GameName = "CallOfDuty",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        var paginatedQuery = new PaginatedQuery
        {
            PageNumber = 1,
            PageSize = 5
        };
        var paginatedData = new PaginatedResults<GameDto>
        {
            Results =
            [
                new GameDto
                {
                    GameId = "1",
                    GameName = "Battlefield",
                    CreatedDate = new DateTime(2026, 02, 01),
                    UpdatedDate = new DateTime(2026, 02, 02),
                    YearPlayed = new DateTime(2026, 02, 01),
                    CategoryId = "1",
                    CreatedBy = "Piotr",
                    UpdatedBy = "Piotr"
                },
                new GameDto
                {
                    GameId = "2",
                    GameName = "CallOfDuty",
                    CreatedDate = new DateTime(2026, 02, 01),
                    UpdatedDate = new DateTime(2026, 02, 02),
                    YearPlayed = new DateTime(2026, 02, 01),
                    CategoryId = "1",
                    CreatedBy = "Piotr",
                    UpdatedBy = "Piotr"
                }
            ],
            TotalAmount = 2,
            PageNumber = 1,
            PageSize = 5,
            FirstItemIndexList = 1,
            LastItemIndexList = 2,
            AmountPagesList = [1]
        };
        utilsServiceMock.Setup(x => x.GetPaginatedData(It.IsAny<IQueryable<GameDto>>(), It.IsAny<PaginatedQuery>()))
            .Returns(Task.FromResult(paginatedData));
        await gameLogDbContextMock.Games.AddRangeAsync(gamesMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken);

        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = await gameService.GetGames("1", paginatedQuery);

        //Assert
        result.Results.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetGame_ForValidGameId_ReturnGame()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gamesMock = new List<Games>
        {
            new()
            {
                GameId = "1",
                GameName = "Battlefield",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            },
            new()
            {
                GameId = "2",
                GameName = "CallOfDuty",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        var categoryMock = new Categories
        {
            CategoryId = "1",
            CategoryName = "FPS",
            Description = "Shooting games",
            CreatedDate = new DateTime(2026, 01, 01),
            UpdatedDate = new DateTime(2026, 01, 02),
            CreatedBy = "Jakub",
            UpdatedBy = "Jakub",
            UserId = "1"
        };
        await gameLogDbContextMock.AddAsync(categoryMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.AddRangeAsync(gamesMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken);

        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = await gameService.GetGame("1");

        //Assert
        result.GameName.Should().Be("Battlefield");
    }

    [Fact]
    public async Task GetGame_ForInvalidGameId_ThrowNotFoundException()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gamesMock = new List<Games>
        {
            new()
            {
                GameId = "1",
                GameName = "Battlefield",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            },
            new()
            {
                GameId = "2",
                GameName = "CallOfDuty",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        var categoryMock = new Categories
        {
            CategoryId = "1",
            CategoryName = "FPS",
            Description = "Shooting games",
            CreatedDate = new DateTime(2026, 01, 01),
            UpdatedDate = new DateTime(2026, 01, 02),
            CreatedBy = "Jakub",
            UpdatedBy = "Jakub",
            UserId = "1"
        };
        await gameLogDbContextMock.AddAsync(categoryMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.AddRangeAsync(gamesMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken);

        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = async () => await gameService.GetGame("3");

        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Game not found");
    }

    [Fact]
    public async Task PostGame_ForValidData_ReturnsAddedGame()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var newGame = new GamePostDto
        {
            GameName = "Fortnite",
            CategoryId = "1",
            YearPlayed = new DateTime(2026, 02, 01)
        };
        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = await gameService.PostGame(newGame, "1");
        var countGames = await gameLogDbContextMock.Games.CountAsync(TestContext.Current.CancellationToken);

        //Assert
        result.GameName.Should().Be("Fortnite");
        countGames.Should().Be(1);
    }

    [Fact]
    public async Task PostGame_ForInvalidGameName_ThrowBadRequestException()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gameMock = new Games
        {
            GameId = "1",
            GameName = "Fortnite",
            CreatedDate = new DateTime(2026, 02, 01),
            UpdatedDate = new DateTime(2026, 02, 02),
            YearPlayed = new DateTime(2026, 02, 01),
            CategoryId = "1",
            CreatedBy = "Piotr",
            UpdatedBy = "Piotr",
            UserId = "1"
        };
        var newGame = new GamePostDto
        {
            GameName = "Fortnite",
            CategoryId = "1",
            YearPlayed = new DateTime(2026, 02, 01)
        };
        await gameLogDbContextMock.AddAsync(gameMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken); //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = async () => await gameService.PostGame(newGame, "1");
        var countGames = await gameLogDbContextMock.Games.CountAsync(TestContext.Current.CancellationToken);

        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Game with this name already exist");
    }

    [Fact]
    public async Task PutGame_ForValidGame_ShouldUpdateGame()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gamesMock = new List<Games>
        {
            new()
            {
                GameId = "1",
                GameName = "Fortnite",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            },
            new()
            {
                GameId = "2",
                GameName = "CallOfDuty",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        var updatedGame = new GamePutDto
        {
            GameName = "Fortnite",
            CategoryId = "1",
            YearPlayed = new DateTime(2026, 02, 01)
        };
        await gameLogDbContextMock.AddRangeAsync(gamesMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = await gameService.PutGame(updatedGame, "1", "1");

        //Assert
        result.GameId.Should().Be("1");
        result.GameName.Should().Be("Fortnite");
    }

    [Fact]
    public async Task PutGame_ForInvalidGameId_ThrowNotFoundException()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gamesMock = new List<Games>
        {
            new()
            {
                GameId = "1",
                GameName = "Fortnite",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            },
            new()
            {
                GameId = "2",
                GameName = "CallOfDuty",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        var updatedGame = new GamePutDto
        {
            GameName = "Fortnite",
            CategoryId = "1",
            YearPlayed = new DateTime(2026, 02, 01)
        };
        await gameLogDbContextMock.AddRangeAsync(gamesMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = async () => await gameService.PutGame(updatedGame, "4", "1");

        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Game not found");
    }

    [Fact]
    public async Task PutGame_ForExistGameName_BadRequestException()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gamesMock = new List<Games>
        {
            new()
            {
                GameId = "1",
                GameName = "Fortnite",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            },
            new()
            {
                GameId = "2",
                GameName = "CallOfDuty",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        var updatedGame = new GamePutDto
        {
            GameName = "Fortnite",
            CategoryId = "1",
            YearPlayed = new DateTime(2026, 02, 01)
        };
        await gameLogDbContextMock.AddRangeAsync(gamesMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = async () => await gameService.PutGame(updatedGame, "2", "1");

        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Game with this name already exist");
    }

    [Fact]
    public async Task DeleteGame_ForValidGameId_ShouldDeleteGame()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gamesMock = new List<Games>
        {
            new()
            {
                GameId = "1",
                GameName = "Fortnite",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            },
            new()
            {
                GameId = "2",
                GameName = "CallOfDuty",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        await gameLogDbContextMock.AddRangeAsync(gamesMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        await gameService.DeleteGame("2", "1");
        var result = await gameLogDbContextMock.Games.CountAsync(TestContext.Current.CancellationToken);
        //Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task DeleteGame_ForInvalidGameId_ThrowNotFoundException()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gamesMock = new List<Games>
        {
            new()
            {
                GameId = "1",
                GameName = "Fortnite",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            },
            new()
            {
                GameId = "2",
                GameName = "CallOfDuty",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        await gameLogDbContextMock.AddRangeAsync(gamesMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken);
        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = async () => await gameService.DeleteGame("3", "1");
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Game not found");
    }

    [Fact]
    public async Task GetGamesByUserId_ForValidUserId_ReturnListGames()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gamesMock = new List<Games>
        {
            new()
            {
                GameId = "1",
                GameName = "Fortnite",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            },
            new()
            {
                GameId = "2",
                GameName = "CallOfDuty",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        await gameLogDbContextMock.AddRangeAsync(gamesMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken);

        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = await gameService.GetGamesByUserId("1");

        //Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetGamesByCategoryId_ForValidCategoryId_ReturnListGames()
    {
        //Arrange
        await using var gameLogDbContextMock = GetDbContext();
        var utilsServiceMock = new Mock<IUtilsService>();
        var gamesMock = new List<Games>
        {
            new()
            {
                GameId = "1",
                GameName = "Fortnite",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            },
            new()
            {
                GameId = "2",
                GameName = "CallOfDuty",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "2",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1"
            }
        };
        await gameLogDbContextMock.AddRangeAsync(gamesMock, TestContext.Current.CancellationToken);
        await gameLogDbContextMock.SaveChangesAsync(TestContext.Current.CancellationToken);

        //Act
        var gameService = new GameService(gameLogDbContextMock, utilsServiceMock.Object);
        var result = await gameService.GetGamesByCategoryId("1");

        //Assert
        result.Should().HaveCount(1);
    }
}