using GameLogBack.DbContext;
using GameLogBack.Dtos.Game;
using GameLogBack.Entities;
using GameLogBack.Exceptions;
using GameLogBack.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.Services;

public class GameService : IGameService
{
    private readonly GameLogDbContext _context;

    public GameService(GameLogDbContext context)
    {
        _context = context;
    }

    public IEnumerable<GameDto> GetGames(string userId)
    {
        var isUerExist = _context.Users.Any(x => x.UserId == userId);
        if (!isUerExist)
        {
            throw new BadRequestException("User not found");
        }

        var games = _context.Games.Include(x => x.Category).Where(x => x.UserId == userId).Select(x =>
            new GameDto()
            {
                GameId = x.GameId,
                GameName = x.GameName,
                UpdatedDate = x.UpdatedDate,
                UpdatedBy = x.UpdatedBy,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.CategoryName,
            }
            ).ToList();
        return games;
    }

    public GameDto GetGame(string gameId)
    {
        var game = _context.Games.Include(x => x.Category).Where(x => x.GameId == gameId).Select(x => new GameDto()
        {
            GameId = x.GameId,
            GameName = x.GameName,
            UpdatedDate = x.UpdatedDate,
            UpdatedBy = x.UpdatedBy,
            CreatedDate = x.CreatedDate,
            CreatedBy = x.CreatedBy,
            CategoryId = x.CategoryId,
            CategoryName = x.Category.CategoryName,       
        }).FirstOrDefault();
        if (game is null)
        {
            throw new BadRequestException("Game not found");
        }
        return game;
    }

    public GameDto PostGame(GamePostDto gamePostDto, string userId)
    {
        var isUserExist = _context.Users.Any(x => x.UserId == userId);
        if (!isUserExist)
        {
            throw new BadRequestException("User not found");
        }

        var isGameNameExist = _context.Games.Any(x => x.GameName == gamePostDto.GameName && x.UserId == userId);
        if (isGameNameExist)
        {
            throw new BadRequestException("Game with this name already exist");
        }
        var newGame = new Games()
        {
            GameId = Guid.NewGuid().ToString(),
            GameName = gamePostDto.GameName,
            UserId = userId,
            CategoryId = gamePostDto.CategoryId,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedBy = null,
        };
        _context.Games.Add(newGame);
        _context.SaveChanges();
        var categoryName = _context.Games.Include(x => x.Category).Where(x => x.GameId == newGame.GameId)
            .Select(x => x.Category.CategoryName).FirstOrDefault();
        return new GameDto()
        {
            GameId = newGame.GameId,
            GameName = newGame.GameName,
            CategoryId = newGame.CategoryId, 
            CategoryName = categoryName,
            CreatedDate = newGame.CreatedDate,
            UpdatedDate = newGame.UpdatedDate,
            CreatedBy = newGame.CreatedBy,
            UpdatedBy = newGame.UpdatedBy
        };
    }

    public GameDto PutGame(GamePutDto gamePutDto, string gameId, string userId)
    {
        var isUerExist = _context.Users.Any(x => x.UserId == userId);
        if (!isUerExist)
        {
            throw new BadRequestException("User not found");
        }
        var isGameExist = _context.Games.Any(x => x.GameId == gameId && x.UserId == userId);
        if (!isGameExist)
        {
            throw new BadRequestException("Game not found");
        }
        var gameToUpdate = _context.Games.Include(x => x.Category).FirstOrDefault(x => x.GameName == gamePutDto.GameName && x.UserId == userId && x.GameId != gameId);
        if (gameToUpdate is null)
        {
            throw new BadRequestException("Game with this name already exist");
        }
        gameToUpdate.GameName = gamePutDto.GameName;
        gameToUpdate.UpdatedBy = userId;
        gameToUpdate.CategoryId = gamePutDto.CategoryId;       
        gameToUpdate.UpdatedDate = DateTime.UtcNow;
        _context.SaveChanges();
        return new GameDto()
        {
            GameId = gameToUpdate.GameId,
            GameName = gamePutDto.GameName,
            CategoryId = gamePutDto.CategoryId, 
            CategoryName = gameToUpdate.Category.CategoryName,
            CreatedBy = gameToUpdate.CreatedBy,
            UpdatedBy = gameToUpdate.UpdatedBy,
            CreatedDate = gameToUpdate.CreatedDate,
            UpdatedDate = gameToUpdate.UpdatedDate
        };
    }
    
    public void DeleteGame(string gameId, string userId)
    {
        var isUerExist = _context.Users.Any(x => x.UserId == userId);
        if (!isUerExist)
        {
            throw new BadRequestException("User not found");
        }
        var gameToDelete = _context.Games.FirstOrDefault(x => x.GameId == gameId && x.UserId == userId);
        if (gameToDelete is null)
        {
            throw new BadRequestException("Game not found");
        }
        _context.Games.Remove(gameToDelete);
        _context.SaveChanges();       
    }
}