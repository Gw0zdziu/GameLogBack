using GameLogBack.DataAccess.Interfaces;
using GameLogBack.DbContext;
using GameLogBack.Dtos.Game.RequestDto;
using GameLogBack.Dtos.Game.ResponseDto;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Services;

public class GameService : IGameService
{
    private readonly IUtilsService _utilsService;   
    private readonly IGameRepository _gameRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IRailwayBucketService _railwayBucketService;

    public GameService(IUtilsService utilsService, IRailwayBucketService railwayBucketService, IGameRepository gameRepository, ICategoryRepository categoryRepository)
    {
        _utilsService = utilsService;
        _railwayBucketService = railwayBucketService;
        _gameRepository = gameRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<PaginatedResults<GameDto>> GetGames(string userId, PaginatedQuery paginatedQuery)
    {
        var games = await _gameRepository.GetByUserId(userId, paginatedQuery);
        var gamesDtoPaginated = new PaginatedResults<GameDto>
        {
            Results = games.Results.Select(x =>
                new GameDto
                {
                    GameId = x.GameId,
                    GameName = x.GameName,
                    GameUrl = _railwayBucketService.FetchFile(x.GameImagePath),
                    UpdatedDate = x.UpdatedDate,
                    UpdatedBy = x.UpdatedBy,
                    CreatedDate = x.CreatedDate,
                    CreatedBy = x.CreatedBy,
                    YearPlayed = x.YearPlayed,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.CategoryName
                }
            ).ToList(),
            TotalAmount = games.TotalAmount,
            PageNumber = games.PageNumber,
            PageSize = games.PageSize,
            FirstItemIndexList = games.FirstItemIndexList,
            LastItemIndexList = games.LastItemIndexList,
            AmountPagesList = games.AmountPagesList
        };
        return gamesDtoPaginated;
    }

    public async Task<GameDto> GetGame(string gameId)
    {
        var game = await _gameRepository.GetById(gameId);
        if (game is null)
        {
            throw new NotFoundException("Game not found");
        }
        return  new GameDto
        {
            GameId = game.GameId,
            GameName = game.GameName,
            GameUrl = _railwayBucketService.FetchFile(game.GameImagePath),
            UpdatedDate = game.UpdatedDate,
            UpdatedBy = game.UpdatedBy,
            CreatedDate = game.CreatedDate,
            YearPlayed = game.YearPlayed,
            CreatedBy = game.CreatedBy,
            CategoryId = game.CategoryId,
            CategoryName = game.Category.CategoryName
        };
        
    }

    public async Task PostGame(GamePostDto gamePostDto, string userId)
    {
        string gameImagePath;
        var isGameNameExist = await _gameRepository.CheckIfGameExists(gamePostDto.GameName, userId);
        if (isGameNameExist) throw new BadRequestException("Game with this name already exist");
        var gameNameKebabCase = _utilsService.ToKebabCase(gamePostDto.GameName);
        if (string.IsNullOrEmpty(gamePostDto.GameImageUrl))
        {
            gameImagePath = null;
        }
        else
        {
            gameImagePath = await _railwayBucketService.UploadFile(userId, gameNameKebabCase, gamePostDto.GameImageUrl);

        }
        var newGame = new Games
        {
            GameId = Guid.NewGuid().ToString(),
            GameName = gamePostDto.GameName,
            GameImagePath = gameImagePath,
            UserId = userId,
            CategoryId = gamePostDto.CategoryId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            YearPlayed = gamePostDto.YearPlayed,
            CreatedBy = userId,
            UpdatedBy = null
        };
        await _gameRepository.Create(newGame);
    }

    public async Task<GameDto> PutGame(GamePutDto gamePutDto, string gameId, string userId)
    {
        var game = await _gameRepository.GetByGameIdAndUserId(gameId, userId);
        if (game is null) throw new NotFoundException("Game not found");
        var isGameNameExist = await _gameRepository.CheckIfExistsWithSameName(gamePutDto.GameName, userId, gameId);
        if (isGameNameExist) throw new BadRequestException("Game with this name already exist");

        game.GameName = gamePutDto.GameName;
        game.GameImagePath = await _railwayBucketService.UploadFile(userId, gameId, gamePutDto.GameImageUrl);
        game.UpdatedBy = userId;
        game.CategoryId = gamePutDto.CategoryId;
        game.YearPlayed = gamePutDto.YearPlayed;
        game.UpdatedDate = DateTime.UtcNow;
        await _gameRepository.Update(game);
        var categoryName = await _categoryRepository.GetCategoryName(gamePutDto.CategoryId);
        return new GameDto
        {
            GameId = game.GameId,
            GameName = gamePutDto.GameName,
            CategoryId = gamePutDto.CategoryId,
            GameUrl = _railwayBucketService.FetchFile(game.GameImagePath),
            CategoryName = categoryName,
            CreatedBy = game.CreatedBy,
            UpdatedBy = game.UpdatedBy,
            YearPlayed = game.YearPlayed,
            CreatedDate = game.CreatedDate,
            UpdatedDate = game.UpdatedDate
        };
    }

    public async Task DeleteGame(string gameId, string userId)
    {
        var gameToDelete = await _gameRepository.GetByGameIdAndUserId(gameId, userId);
        if (gameToDelete is null) throw new NotFoundException("Game not found");
        await _gameRepository.Delete(gameToDelete);
    }

    

    public async Task<IEnumerable<GameByUserIdDto>> GetGamesByUserId(string userId, PaginatedQuery paginatedQuery)
    {
        var categories = await _gameRepository.GetByUserId(userId, paginatedQuery);
        var gamesByUserId = categories.Results.Select(x =>
            new GameByUserIdDto
            {
                GameId = x.GameId,
                GameName = x.GameName,
                UpdatedDate = x.UpdatedDate,
                CreatedDate = x.CreatedDate,
                YearPlayed = x.YearPlayed,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.CategoryName
            }).ToList();
        return gamesByUserId;
    }

    public async Task<IEnumerable<GameByCategoryIdDto>> GetGamesByCategoryId(string categoryId)
    {
        var games = await _gameRepository.GetByCategoryId(categoryId);
        return
        [
            .. games.Select(x =>
                new GameByCategoryIdDto
                {
                    GameId = x.GameId,
                    GameName = x.GameName,
                    UpdatedDate = x.UpdatedDate,
                    CreatedDate = x.CreatedDate,
                    CategoryId = x.CategoryId,
                    YearPlayed = x.YearPlayed,
                    CategoryName = x.Category.CategoryName
                })
        ];
        
    }
}
