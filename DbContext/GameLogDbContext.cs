using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack;

public class GameLogDbContext : DbContext
{
    public GameLogDbContext(DbContextOptions<GameLogDbContext> options) : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
    public DbSet<UserLogins> UserLogins { get; set; }
    
}