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
    public DbSet<InvitationCodes> InvitationCodes { get; set; }



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
            .HasDefaultValue(true);

        #endregion

        #region UserLogins

        modelBuilder.Entity<UserLogins>().ToTable("user_logins")
            .HasKey(k => k.UserId);
        modelBuilder.Entity<UserLogins>(entity =>
        {
            entity.Property(p => p.UserId)
                .HasColumnName("user_id")
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.UserName)
                .HasColumnName("username")
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.Password)
                .HasColumnName("password")
                .IsRequired()
                .HasMaxLength(100);
        });
        modelBuilder.Entity<UserLogins>()
            .HasOne(r => r.User)
            .WithOne(u => u.UserLogins)
            .HasForeignKey<UserLogins>(k => k.UserId);
        #endregion

        #region RefreshTokenInfo

        modelBuilder.Entity<RefreshTokenInfo>().ToTable("refresh_token_info");
        modelBuilder.Entity<RefreshTokenInfo>(entity =>
        {
            entity.Property(p => p.UserId)
                .HasColumnName("user_id")
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.ExpiryDate)
                .HasColumnName("expiry_date")
                .IsRequired();
            entity.Property(p => p.RefreshToken)
                .HasColumnName("refresh_token")
                .HasMaxLength(150);
        });
        modelBuilder.Entity<RefreshTokenInfo>()
            .HasKey(k => k.RefreshToken);
        modelBuilder.Entity<RefreshTokenInfo>()
            .HasOne(r => r.User)
            .WithOne(u => u.RefreshTokens)
            .HasForeignKey<RefreshTokenInfo>(k => k.UserId);

        #endregion

        #region CodeConfirmUsers

        modelBuilder.Entity<CodeConfirmUsers>().ToTable("confirm_confirm_users");
        modelBuilder.Entity<CodeConfirmUsers>(entity =>
        {
            entity.Property(p => p.UserId).HasColumnName("user_id")
                .IsRequired();
            entity.Property(p => p.Code).HasColumnName("code")
                .HasMaxLength(6);
            entity.Property(p => p.ExpiryDate).HasColumnName("expiry_date")
                .IsRequired();
        });
        modelBuilder.Entity<CodeConfirmUsers>()
            .HasKey(k => k.Code);
        modelBuilder.Entity<CodeConfirmUsers>()
            .HasOne(r => r.User)
            .WithOne(r => r.CodeConfirm)
            .HasForeignKey<CodeConfirmUsers>(k => k.UserId);


        #endregion

        #region CodeRecoveryPassword

        modelBuilder.Entity<CodeRecoveryPassword>().ToTable("code_recovery_password");
        modelBuilder.Entity<CodeRecoveryPassword>(entities =>
        {
            entities.Property(p => p.UserId).HasColumnName("user_id")
                .IsRequired();
            entities.Property(p => p.Code).HasColumnName("code")
                .IsRequired();
            entities.Property(p => p.ExpiryDate).HasColumnName("expiry_date")
                .IsRequired();
            entities.Property(x => x.IsUsed).HasColumnName("is_used");
        });
        modelBuilder.Entity<CodeRecoveryPassword>()
            .HasKey(k => k.Code);
        modelBuilder.Entity<CodeRecoveryPassword>()
            .HasOne(r => r.User)
            .WithOne(r => r.CodeRecoveryPassword)
            .HasForeignKey<CodeRecoveryPassword>(k => k.UserId);


        #endregion

        #region Categories
        
        modelBuilder.Entity<Categories>()
            .ToTable("categories");
        modelBuilder.Entity<Categories>(entity =>
        {
            entity.Property(p => p.CategoryId).HasColumnName("category_id")
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.CategoryName).HasColumnName("category_name")
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.CreatedBy).HasColumnName("created_by");
            entity.Property(p => p.UpdatedBy).HasColumnName("updated_by");
            entity.Property(p => p.CreatedDate).HasColumnName("created_date");
            entity.Property(p => p.UpdatedDate).HasColumnName("updated_date");
            entity.Property(p => p.Description).HasColumnName("description");
            entity.Property(p => p.UserId).HasColumnName("user_id")
                .IsRequired();
        });
        modelBuilder.Entity<Categories>()
            .HasKey(k => k.CategoryId);

        #endregion

        #region Games

        modelBuilder.Entity<Games>()
            .ToTable("games");
        modelBuilder.Entity<Games>(entity =>
        {
            entity.Property(p => p.GameId).HasColumnName("game_id")
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.GameName).HasColumnName("game_name")
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(p => p.CreatedDate).HasColumnName("created_date");
            entity.Property(p => p.CreatedBy).HasColumnName("created_by");
            entity.Property(p => p.UpdatedDate).HasColumnName("updated_date");
            entity.Property(p => p.UpdatedBy).HasColumnName("updated_by");
            entity.Property(p => p.CategoryId).HasColumnName("category_id");
            entity.Property(p => p.UserId).HasColumnName("user_id");
            entity.Property(p => p.YearPlayed).HasColumnName("year_played");
            entity.Property(p => p.GameImagePath).HasColumnName("game_image_path")
                .IsRequired(false);
        });
        modelBuilder.Entity<Games>()
            .HasKey(k => k.GameId);
        #endregion

        #region InvitationCodes

        modelBuilder.Entity<InvitationCodes>().ToTable("invitation_codes");
        modelBuilder.Entity<InvitationCodes>().HasKey(k => k.InvitationCode);
        modelBuilder.Entity<InvitationCodes>(entity =>
        {
            entity.Property(x => x.InvitationCode)
                .HasMaxLength(6)
                .HasColumnName("invitation_code");
            entity.Property(x => x.IsUsed).HasColumnName("is_used");
        });

        #endregion

    }


}
