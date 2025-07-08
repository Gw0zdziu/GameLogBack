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
    
    public DbSet<RefreshTokenInfo> RefreshTokens { get; set; }
    
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
        modelBuilder.Entity<Users>()
            .Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(false);

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

        #region RefreshTokenInfo

        modelBuilder.Entity<RefreshTokenInfo>().ToTable("refresh_token_info");
        modelBuilder.Entity<RefreshTokenInfo>(entity =>
        {
            entity.Property(p => p.UserId).HasColumnName("user_id");
            entity.Property(p => p.RefreshTokenId).HasColumnName("refresh_token_id");
            entity.Property(p => p.ExpiryDate).HasColumnName("expiry_date");
            entity.Property(p => p.RefreshToken).HasColumnName("refresh_token");
        });
        modelBuilder.Entity<RefreshTokenInfo>()
            .HasKey(k => k.RefreshTokenId);
        modelBuilder.Entity<RefreshTokenInfo>()
            .HasOne(r => r.User)
            .WithOne(u => u.RefreshTokens)
            .HasForeignKey<RefreshTokenInfo>(k => k.UserId);
        modelBuilder.Entity<RefreshTokenInfo>()
            .Property(p => p.RefreshTokenId)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<RefreshTokenInfo>()
            .Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<RefreshTokenInfo>()
            .Property(p => p.ExpiryDate)
            .IsRequired();
        modelBuilder.Entity<RefreshTokenInfo>()
            .Property(p => p.RefreshToken)
            .IsRequired();

        #endregion

        #region ConfirmCodeUsers
        
        modelBuilder.Entity<ConfirmCodeUsers>().ToTable("confirm_code_users");
        modelBuilder.Entity<ConfirmCodeUsers>(entity =>
        {
            entity.Property(p => p.UserId).HasColumnName("user_id");
            entity.Property(p => p.ConfirmCode).HasColumnName("confirm_code");
            entity.Property(p => p.ConfirmCodeId).HasColumnName("confirm_code_id");
            entity.Property(p => p.ExpiryDate).HasColumnName("expiry_date");
        });
        modelBuilder.Entity<ConfirmCodeUsers>()
            .HasKey(k => k.ConfirmCodeId);
        modelBuilder.Entity<ConfirmCodeUsers>()
            .HasOne(r => r.User)
            .WithOne(r => r.ConfirmCode)
            .HasForeignKey<ConfirmCodeUsers>(k => k.UserId);
        modelBuilder.Entity<ConfirmCodeUsers>()
            .Property(p => p.ConfirmCodeId)
            .IsRequired();
        modelBuilder.Entity<ConfirmCodeUsers>()
            .Property(p => p.ConfirmCode)
            .IsRequired()
            .HasMaxLength(4);
        modelBuilder.Entity<ConfirmCodeUsers>()
            .Property(p => p.UserId)
            .IsRequired();
        modelBuilder.Entity<ConfirmCodeUsers>()
            .Property(p => p.ExpiryDate)
            .IsRequired();

        #endregion

    }
    
    
}