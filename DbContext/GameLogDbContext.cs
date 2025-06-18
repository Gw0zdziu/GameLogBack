using GameLogBack.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameLogBack.DbContext;

public class GameLogDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public GameLogDbContext(DbContextOptions<GameLogDbContext> options) : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
    public DbSet<UserLogins> UserLogins { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Users
        
        modelBuilder.Entity<Users>().ToTable("users");
        modelBuilder.Entity<Users>(entity =>
        {
            entity.Property(p => p.UserId).HasColumnName("user_id");
            entity.Property(p => p.UserEmail).HasColumnName("user_email");
            entity.Property(p => p.FirstName).HasColumnName("firstname");
            entity.Property(p => p.LastName).HasColumnName("lastname");
        });
        modelBuilder.Entity<Users>()
            .HasKey(k => k.UserId);
        modelBuilder.Entity<Users>()
            .Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<Users>()
            .Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<Users>()
            .Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        #endregion

        #region UserLogins

        modelBuilder.Entity<UserLogins>().ToTable("user_logins");
        modelBuilder.Entity<UserLogins>(entity =>
        {
            entity.Property(p => p.UserId).HasColumnName("user_id");
            entity.Property(p => p.UserName).HasColumnName("username");
            entity.Property(p => p.Password).HasColumnName("password");
        });
        modelBuilder.Entity<UserLogins>()
            .HasKey(k => k.UserId);
        modelBuilder.Entity<UserLogins>()
            .HasOne(r => r.User)
            .WithOne(u => u.UserLogins)
            .HasForeignKey<UserLogins>(k => k.UserId);
        modelBuilder.Entity<UserLogins>()
            .Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<UserLogins>()
            .Property(p => p.UserName)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<UserLogins>()
            .Property(p => p.Password)
            .IsRequired()
            .HasMaxLength(100);
        #endregion
    }
    
    
}