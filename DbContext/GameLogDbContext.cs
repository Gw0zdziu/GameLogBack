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
    public DbSet<CodeConfirmUsers> CodeConfirmUsers { get; set; }
    public DbSet<CodeRecoveryPassword> CodeRecoveryPasswords { get; set; }
    public DbSet<Categories> Categories { get; set; }
    public DbSet<Games> Games { get; set; }
    
    
    
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
            entity.Property(p => p.IsActive).HasColumnName("is_active");
        });
        modelBuilder.Entity<Users>()
            .HasKey(k => k.UserId);
        modelBuilder.Entity<Users>()
            .Property(p => p.UserId)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<Users>()
            .Property(p => p.UserEmail)
            .IsRequired();
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

        #region CodeConfirmUsers
        
        modelBuilder.Entity<CodeConfirmUsers>().ToTable("confirm_confirm_users");
        modelBuilder.Entity<CodeConfirmUsers>(entity =>
        {
            entity.Property(p => p.UserId).HasColumnName("user_id");
            entity.Property(p => p.Code).HasColumnName("confirm_code");
            entity.Property(p => p.CodeId).HasColumnName("confirm_code_id");
            entity.Property(p => p.ExpiryDate).HasColumnName("expiry_date");
        });
        modelBuilder.Entity<CodeConfirmUsers>()
            .HasKey(k => k.CodeId);
        modelBuilder.Entity<CodeConfirmUsers>()
            .HasOne(r => r.User)
            .WithOne(r => r.CodeConfirm)
            .HasForeignKey<CodeConfirmUsers>(k => k.UserId);
        modelBuilder.Entity<CodeConfirmUsers>()
            .Property(p => p.CodeId)
            .IsRequired();
        modelBuilder.Entity<CodeConfirmUsers>()
            .Property(p => p.Code)
            .HasMaxLength(4);
        modelBuilder.Entity<CodeConfirmUsers>()
            .Property(p => p.UserId)
            .IsRequired();
        modelBuilder.Entity<CodeConfirmUsers>()
            .Property(p => p.ExpiryDate)
            .IsRequired();

        #endregion

        #region CodeRecoveryPassword

        modelBuilder.Entity<CodeRecoveryPassword>().ToTable("code_recovery_password");
        modelBuilder.Entity<CodeRecoveryPassword>(entities =>
        {
            entities.Property(p => p.UserId).HasColumnName("user_id");
            entities.Property(p => p.Code).HasColumnName("recovery_code");
            entities.Property(p => p.CodeId).HasColumnName("recovery_code_id");
            entities.Property(p => p.ExpiryDate).HasColumnName("expiry_date");
        });
        modelBuilder.Entity<CodeRecoveryPassword>()
            .HasKey(k => k.CodeId);
        modelBuilder.Entity<CodeRecoveryPassword>()
            .HasOne(r => r.User)
            .WithOne(r => r.CodeRecoveryPassword)
            .HasForeignKey<CodeRecoveryPassword>(k => k.UserId);
        modelBuilder.Entity<CodeRecoveryPassword>()
            .Property(p => p.CodeId)
            .IsRequired();
        modelBuilder.Entity<CodeRecoveryPassword>()
            .Property(p => p.Code);
        modelBuilder.Entity<CodeRecoveryPassword>()
            .Property(p => p.UserId)
            .IsRequired();
        modelBuilder.Entity<CodeRecoveryPassword>()
            .Property(p => p.ExpiryDate)
            .IsRequired();
        

        #endregion
        
        #region Categories
        modelBuilder.Entity<Categories>()
            .ToTable("categories");
        modelBuilder.Entity<Categories>(entity =>
        {
            entity.Property(p => p.CategoryId).HasColumnName("category_id");
            entity.Property(p => p.CategoryName).HasColumnName("category_name");
            entity.Property(p => p.CreatedBy).HasColumnName("created_by");
            entity.Property(p => p.UpdatedBy).HasColumnName("updated_by");
            entity.Property(p => p.CreatedDate).HasColumnName("created_date");
            entity.Property(p => p.UpdatedDate).HasColumnName("updated_date");
            entity.Property(p => p.Description).HasColumnName("description");
            entity.Property(p => p.UserId).HasColumnName("user_id");
        });
        modelBuilder.Entity<Categories>()
            .HasKey(k => k.CategoryId);
        modelBuilder.Entity<Categories>()
            .Property(p => p.CategoryId)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<Categories>()
            .Property(p => p.CategoryName)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<Categories>()
            .Property(p => p.UserId)
            .IsRequired();

        #endregion
        
        #region Games
        
        modelBuilder.Entity<Games>()
            .ToTable("games");
        modelBuilder.Entity<Games>(entity =>
        {
            entity.Property(p => p.GameId).HasColumnName("game_id");
            entity.Property(p => p.GameName).HasColumnName("game_name");
            entity.Property(p => p.CreatedDate).HasColumnName("created_date");
            entity.Property(p => p.CreatedBy).HasColumnName("created_by");
            entity.Property(p => p.UpdatedDate).HasColumnName("updated_date");
            entity.Property(p => p.UpdatedBy).HasColumnName("updated_by");
            entity.Property(p => p.CategoryId).HasColumnName("category_id");
            entity.Property(p => p.UserId).HasColumnName("user_id");
        });
        modelBuilder.Entity<Games>()
            .HasKey(k => k.GameId);
        modelBuilder.Entity<Games>()
            .Property(p => p.GameId)
            .IsRequired()
            .HasMaxLength(100);
        modelBuilder.Entity<Games>()
            .Property(p => p.GameName)
            .IsRequired()
            .HasMaxLength(100);
        #endregion

    }
    
    
}