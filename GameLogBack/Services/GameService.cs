using GameLogBack.DbContext;
using GameLogBack.Dtos.Game;
using GameLogBack.Dtos.PaginatedQuery;
using GameLogBack.Dtos.PaginatedResults;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Services;

public class GameService : IGameService
{
    private readonly GameLogDbContext _context;
    private readonly IUtilsService _utilsService;

    public GameService(GameLogDbContext context, IUtilsService utilsService)
    {
        _context = context;
        _utilsService = utilsService;
    }

    public async Task<PaginatedResults<GameDto>> GetGames(string userId, PaginatedQuery paginatedQuery)
    {
        var games = _context.Games.Include(x => x.Category).Where(x => x.UserId == userId).Select(x =>
            new GameDto
            {
                GameId = x.GameId,
                GameName = x.GameName,
                UpdatedDate = x.UpdatedDate,
                UpdatedBy = x.UpdatedBy,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                YearPlayed = x.YearPlayed,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.CategoryName
            }
        );
        var gamesPaginated = await _utilsService.GetPaginatedData(games, paginatedQuery);
        return gamesPaginated;
    }

    public async Task<GameDto> GetGame(string gameId)
    {
        var game = await _context.Games.Include(x => x.Category).Where(x => x.GameId == gameId).Select(x => new GameDto
        {
            GameId = x.GameId,
            GameName = x.GameName,
            UpdatedDate = x.UpdatedDate,
            UpdatedBy = x.UpdatedBy,
            CreatedDate = x.CreatedDate,
            YearPlayed = x.YearPlayed,
            CreatedBy = x.CreatedBy,
            CategoryId = x.CategoryId,
            CategoryName = x.Category.CategoryName
        }).FirstOrDefaultAsync();
        return game ?? throw new NotFoundException("Game not found");
    }

    public async Task<GameDto> PostGame(GamePostDto gamePostDto, string userId)
    {
        var isGameNameExist =
            await _context.Games.AnyAsync(x => x.UserId == userId && x.GameName == gamePostDto.GameName);
        if (isGameNameExist) throw new BadRequestException("Game with this name already exist");
        var newGame = new Games
        {
            GameId = Guid.NewGuid().ToString(),
            GameName = gamePostDto.GameName,
            UserId = userId,
            CategoryId = gamePostDto.CategoryId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            YearPlayed = gamePostDto.YearPlayed,
            CreatedBy = userId,
            UpdatedBy = null
        };
        _context.Games.Add(newGame);
        await _context.SaveChangesAsync();
        var categoryName = await _context.Categories.Where(c => c.CategoryId == gamePostDto.CategoryId)
            .Select(c => c.CategoryName).FirstOrDefaultAsync();
        return new GameDto
        {
            GameId = newGame.GameId,
            GameName = newGame.GameName,
            CategoryId = newGame.CategoryId,
            CategoryName = categoryName,
            CreatedDate = newGame.CreatedDate,
            UpdatedDate = newGame.UpdatedDate,
            YearPlayed = newGame.YearPlayed,
            CreatedBy = newGame.CreatedBy,
            UpdatedBy = newGame.UpdatedBy
        };
    }

    public async Task<GameDto> PutGame(GamePutDto gamePutDto, string gameId, string userId)
    {
        var game = await _context.Games.FirstOrDefaultAsync(x =>
            x.UserId == userId && x.GameId == gameId);
        if (game is null) throw new NotFoundException("Game not found");
        var isGameNameExist = await _context.Games.AnyAsync(x =>
            x.UserId == userId && x.GameId != gameId && x.GameName == gamePutDto.GameName);
        if (isGameNameExist) throw new BadRequestException("Game with this name already exist");

        game.GameName = gamePutDto.GameName;
        game.UpdatedBy = userId;
        game.CategoryId = gamePutDto.CategoryId;
        game.YearPlayed = gamePutDto.YearPlayed;
        game.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        var categoryName = await _context.Categories.Where(c => c.CategoryId == game.CategoryId)
            .Select(c => c.CategoryName).FirstOrDefaultAsync();
        return new GameDto
        {
            GameId = game.GameId,
            GameName = gamePutDto.GameName,
            CategoryId = gamePutDto.CategoryId,
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
        var gameToDelete = _context.Games.FirstOrDefault(x => x.GameId == gameId && x.UserId == userId);
        if (gameToDelete is null) throw new BadRequestException("Game not found");
        _context.Games.Remove(gameToDelete);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<GameByUserIdDto>> GetGamesByUserId(string userId)
    {
        var games = await _context.Games.Include(x => x.Category).Where(x => x.UserId == userId).Select(x =>
            new GameByUserIdDto
            {
                GameId = x.GameId,
                GameName = x.GameName,
                UpdatedDate = x.UpdatedDate,
                CreatedDate = x.CreatedDate,
                YearPlayed = x.YearPlayed,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.CategoryName
            }).ToListAsync();
        return games;
    }

    public async Task<IEnumerable<GameByCategoryIdDto>> GetGamesByCategoryId(string categoryId)
    {
        var games = await _context.Games.Include(x => x.Category).Where(x => x.CategoryId == categoryId).Select(x =>
            new GameByCategoryIdDto
            {
                GameId = x.GameId,
                GameName = x.GameName,
                UpdatedDate = x.UpdatedDate,
                CreatedDate = x.CreatedDate,
                CategoryId = x.CategoryId,
                YearPlayed = x.YearPlayed,
                CategoryName = x.Category.CategoryName
            }).ToListAsync();
        return games;
    }
}