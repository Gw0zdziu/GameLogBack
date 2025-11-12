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

    public async Task<IEnumerable<GameDto>> GetGames(string userId)
    {
        var isUerExist = await _context.Users.AnyAsync(x => x.UserId == userId);
        if (!isUerExist)
        {
            throw new BadRequestException("User not found");
        }

        var games = await _context.Games.Include(x => x.Category).Where(x => x.UserId == userId).Select(x =>
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
            ).ToListAsync();
        return games;
    }

    public async Task<GameDto> GetGame(string gameId)
    {
        var game = await _context.Games.Include(x => x.Category).Where(x => x.GameId == gameId).Select(x => new GameDto()
        {
            GameId = x.GameId,
            GameName = x.GameName,
            UpdatedDate = x.UpdatedDate,
            UpdatedBy = x.UpdatedBy,
            CreatedDate = x.CreatedDate,
            CreatedBy = x.CreatedBy,
            CategoryId = x.CategoryId,
            CategoryName = x.Category.CategoryName,       
        }).FirstOrDefaultAsync();
        return game ?? throw new NotFoundException("Game not found");
    }

    public async Task<GameDto> PostGame(GamePostDto gamePostDto, string userId)
    {
        var data = await _context.Users
            .Where(u => u.UserId == userId && u.IsActive)
            .Select(u => new
            {   
                GameExists = u.Games.Any(g => g.GameName == gamePostDto.GameName),
                CategoryName = u.Categories
                    .Where(c => c.CategoryId == gamePostDto.CategoryId)
                    .Select(c => c.CategoryName).FirstOrDefault()
            })
            .FirstOrDefaultAsync();
        if (data is null)
        {
            throw new NotFoundException("User not found");
        }

        if (data.GameExists)
        {
            throw new BadRequestException("Game with this name already exist");
        }
        if (data.CategoryName is null)
        {
            throw new NotFoundException("Category not found");       
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
        await _context.SaveChangesAsync();
        return new GameDto()
        {
            GameId = newGame.GameId,
            GameName = newGame.GameName,
            CategoryId = newGame.CategoryId, 
            CategoryName = data.CategoryName,
            CreatedDate = newGame.CreatedDate,
            UpdatedDate = newGame.UpdatedDate,
            CreatedBy = newGame.CreatedBy,
            UpdatedBy = newGame.UpdatedBy
        };
    }

    public async Task<GameDto> PutGame(GamePutDto gamePutDto, string gameId, string userId)
    {
        var data = await _context.Users.Where(u => u.UserId == userId).Select(u => new
        {
            IsGameNameExist = u.Games.Any(g => (g.UserId == u.UserId && g.GameId != gameId) && g.GameName == gamePutDto.GameName),
            gameToUpdate = u.Games.FirstOrDefault(x =>  x.GameId == gameId),
            CategoryName = u.Categories.Where(c => c.CategoryId == gamePutDto.CategoryId).Select(c => c.CategoryName).FirstOrDefault()
        }).FirstOrDefaultAsync();
        if (data is null)
        {
            throw new NotFoundException("User not found");
        }
        if (data.gameToUpdate is null)
        {
            throw new NotFoundException("Game not found");
        }
        if (data.IsGameNameExist)
        {
            throw new BadRequestException("Game with this name already exist");
        }
        data.gameToUpdate.GameName = gamePutDto.GameName;
        data.gameToUpdate.UpdatedBy = userId;
        data.gameToUpdate.CategoryId = gamePutDto.CategoryId;       
        data.gameToUpdate.UpdatedDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return new GameDto()
        {
            GameId = data.gameToUpdate.GameId,
            GameName = gamePutDto.GameName,
            CategoryId = gamePutDto.CategoryId, 
            CategoryName = data.CategoryName,
            CreatedBy = data.gameToUpdate.CreatedBy,
            UpdatedBy = data.gameToUpdate.UpdatedBy,
            CreatedDate = data.gameToUpdate.CreatedDate,
            UpdatedDate = data.gameToUpdate.UpdatedDate
        };
    }
    
    public async Task DeleteGame(string gameId, string userId)
    {
        var isUerExist = await _context.Users.AnyAsync(x => x.UserId == userId);
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
        await _context.SaveChangesAsync();       
    }
}