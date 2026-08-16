using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using GameLogBack.DataAccess.Interfaces;
using GameLogBack.Dtos.Game.RequestDto;
using GameLogBack.Dtos.Game.ResponseDto;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using GameLogBack.Services;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace GameLogBack.Tests.Services;

[TestSubject(typeof(GameService))]
public class GameServiceTests
{
    [Fact]
    public async Task GetGames_ForValidData_ReturnsCorrectData()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
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
                UserId = "1",
                Category = new Categories()
                {
                    CategoryName = "FPS"
                }
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
                UserId = "1",
                Category = new Categories()
                {
                    CategoryName = "FPS"
                }
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
        mockUtilsService.Setup(x => x.GetPaginatedData(It.IsAny<List<GameDto>>(), It.IsAny<PaginatedQuery>()))
            .Returns(paginatedData);
        mockGameRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(gamesMock);
        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        var result = await gameService.GetGames("1", paginatedQuery);

        //Assert
        result.Results.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetGame_ForValidGameId_ReturnGame()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
        var gameMock = new Games()
        {
            GameId = "1",
            GameName = "CallOfDuty",
            CreatedDate = new DateTime(2026, 02, 01),
            UpdatedDate = new DateTime(2026, 02, 02),
            YearPlayed = new DateTime(2026, 02, 01),
            CategoryId = "1",
            CreatedBy = "Piotr",
            UpdatedBy = "Piotr",
            UserId = "1",
            Category = new Categories()
            {
                CategoryName = "FPS"
            }
        };
        mockGameRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(gameMock);
        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        var result = await gameService.GetGame("1");

        //Assert
        result.GameName.Should().Be("CallOfDuty");
    }

    [Fact]
    public async Task GetGame_ForInvalidGameId_ThrowNotFoundException()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
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
        mockGameRepository.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(null as Games);

        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        var result = async () => await gameService.GetGame("3");

        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Game not found");
    }

    [Fact]
    public async Task PostGame_ForValidData_ReturnsAddedGame()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
        var newGame = new GamePostDto
        {
            GameName = "Fortnite",
            CategoryId = "1",
            YearPlayed = new DateTime(2026, 02, 01)
        };
        mockGameRepository.Setup(x => x.CheckIfGameExists(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        mockRailwayService.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("https://www.google.com");
        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        await gameService.PostGame(newGame, "1");

        //Assert
        mockGameRepository.Verify(x => x.Create(It.IsAny<Games>()), Times.Once);
    }

    [Fact]
    public async Task PostGame_ForInvalidGameName_ThrowBadRequestException()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
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
        mockGameRepository.Setup(x => x.CheckIfGameExists(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        var result = async () => await gameService.PostGame(newGame, "1");

        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Game with this name already exist");
    }

    [Fact]
    public async Task PutGame_ForValidGame_ShouldUpdateGame()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
        var game = new Games()
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
        };
        var updatedGame = new GamePutDto
        {
            GameName = "Fortnite",
            CategoryId = "1",
            YearPlayed = new DateTime(2026, 02, 01)
        };
        mockGameRepository.Setup(x => x.GetByGameIdAndUserId(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(game);
        mockGameRepository
            .Setup(x => x.CheckIfExistsWithSameName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);
        mockRailwayService.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("urlForImageGame");
        mockRailwayService.Setup(x => x.FetchFile(It.IsAny<string>())).Returns("urlForImageGameFromFetch");
        mockCategoryRepository.Setup(x => x.GetCategoryName(It.IsAny<string>())).ReturnsAsync("FPS");

        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        var result = await gameService.PutGame(updatedGame, "1", "1");

        //Assert
        result.GameId.Should().Be("2");
        result.GameName.Should().Be("Fortnite");
    }

    [Fact]
    public async Task PutGame_ForInvalidGameId_ThrowNotFoundException()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
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
        mockGameRepository.Setup(x => x.GetByGameIdAndUserId(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(null as Games);

        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        var result = async () => await gameService.PutGame(updatedGame, "4", "1");

        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Game not found");
    }

    [Fact]
    public async Task PutGame_ForExistGameName_BadRequestException()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
        var game = new Games()
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
        };
        var updatedGame = new GamePutDto
        {
            GameName = "Fortnite",
            CategoryId = "1",
            YearPlayed = new DateTime(2026, 02, 01)
        };
        mockGameRepository.Setup(x => x.GetByGameIdAndUserId(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(game);
        mockGameRepository
            .Setup(x => x.CheckIfExistsWithSameName(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        var result = async () => await gameService.PutGame(updatedGame, "2", "1");

        //Assert
        await result.Should().ThrowAsync<BadRequestException>().WithMessage("Game with this name already exist");
    }

    [Fact]
    public async Task DeleteGame_ForValidGameId_ShouldDeleteGame()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
        var game = new Games()
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
        };
        mockGameRepository.Setup(x => x.GetByGameIdAndUserId(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(game);


        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        await gameService.DeleteGame("2", "1");

        //Assert
        mockGameRepository.Verify(x => x.Delete(It.IsAny<Games>()), Times.Once);
    }

    [Fact]
    public async Task DeleteGame_ForInvalidGameId_ThrowNotFoundException()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
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
        mockGameRepository.Setup(x => x.GetByGameIdAndUserId(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(null as Games);

        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        var result = async () => await gameService.DeleteGame("3", "1");
        //Assert
        await result.Should().ThrowAsync<NotFoundException>().WithMessage("Game not found");
    }

    [Fact]
    public async Task GetGamesByUserId_ForValidUserId_ReturnListGames()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
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
                UserId = "1",
                Category = new Categories()
                {
                    CategoryName = "FPS"
                }
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
                UserId = "1",
                Category = new Categories()
                {
                    CategoryName = "FPS"
                }
            }
        };
        mockGameRepository.Setup(x => x.GetByUserId(It.IsAny<string>())).ReturnsAsync(gamesMock);

        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        var result = await gameService.GetGamesByUserId("1");

        //Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetGamesByCategoryId_ForValidCategoryId_ReturnListGames()
    {
        //Arrange
        var mockUtilsService = new Mock<IUtilsService>();
        var mockCategoryRepository = new Mock<ICategoryRepository>();
        var mockGameRepository = new Mock<IGameRepository>();
        var mockRailwayService = new Mock<IRailwayBucketService>();
        var gameMock = new List<Games>()
        {
            new Games()
            {
                GameId = "1",
                GameName = "Fortnite",
                CreatedDate = new DateTime(2026, 02, 01),
                UpdatedDate = new DateTime(2026, 02, 02),
                YearPlayed = new DateTime(2026, 02, 01),
                CategoryId = "1",
                CreatedBy = "Piotr",
                UpdatedBy = "Piotr",
                UserId = "1",
                Category = new Categories()
                {
                    CategoryName = "FPS"
                }
            }
        };

        mockGameRepository.Setup(x => x.GetByCategoryId(It.IsAny<string>())).ReturnsAsync(gameMock);

        //Act
        var gameService = new GameService(mockUtilsService.Object, mockRailwayService.Object, mockGameRepository.Object,
            mockCategoryRepository.Object);
        var result = await gameService.GetGamesByCategoryId("1");

        //Assert
        result.Should().HaveCount(1);
    }
}