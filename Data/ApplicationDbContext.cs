using System;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Platform> Platforms { get; set; }
        public virtual DbSet<GameGenre> GameGenres { get; set; }
        public virtual DbSet<GamePlatform> GamePlatforms { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderGame> OrderGames { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<RolePermission> RolePermissions { get; set; }

        public ApplicationDbContext() { }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Genre>()
                .HasMany(g => g.SubGenres)
                .WithOne(g => g.ParentGenre)
                .HasForeignKey(g => g.ParentGenreId);

            modelBuilder.Entity<Game>()
                .HasIndex(g => g.Key)
                .IsUnique();
            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();

            modelBuilder.Entity<Platform>()
                .HasIndex(p => p.Type)
                .IsUnique();

            modelBuilder.Entity<GameGenre>()
                .HasKey(gg => new { gg.GameId, gg.GenreId });

            modelBuilder.Entity<GamePlatform>()
                .HasKey(gp => new { gp.GameId, gp.PlatformId });
            
            modelBuilder.Entity<Publisher>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.CompanyName).IsUnique();

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(200);
            });
            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>();

            modelBuilder.Entity<OrderGame>()
                .HasKey(og => new { og.OrderId, og.ProductId });

            modelBuilder.Entity<OrderGame>()
                .HasOne(o => o.Order)
                .WithMany(og => og.OrderGames)
                .HasForeignKey(o => o.OrderId);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany()
                .HasForeignKey(p => p.OrderId);
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);
            
            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);
        

            // Seed Data
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = Guid.NewGuid(), Name = "Strategy" },
                new Genre { Id = Guid.NewGuid(), Name = "RTS", ParentGenreId = Guid.Parse("e8b4576a-070d-4c02-861e-08f6a48ae5e6") },
                new Genre { Id = Guid.NewGuid(), Name = "TBS", ParentGenreId = Guid.Parse("e8b4576a-070d-4c02-861e-08f6a48ae5e6") },
                new Genre { Id = Guid.NewGuid(), Name = "RPG" },
                new Genre { Id = Guid.NewGuid(), Name = "Sports" },
                new Genre { Id = Guid.NewGuid(), Name = "Races", ParentGenreId = Guid.Parse("2f7ef048-5b48-4b61-bb4f-69f89d152342") },
                new Genre { Id = Guid.NewGuid(), Name = "Rally", ParentGenreId = Guid.Parse("2f7ef048-5b48-4b61-bb4f-69f89d152342") },
                new Genre { Id = Guid.NewGuid(), Name = "Arcade", ParentGenreId = Guid.Parse("2f7ef048-5b48-4b61-bb4f-69f89d152342") },
                new Genre { Id = Guid.NewGuid(), Name = "Formula", ParentGenreId = Guid.Parse("2f7ef048-5b48-4b61-bb4f-69f89d152342") },
                new Genre { Id = Guid.NewGuid(), Name = "Off-road", ParentGenreId = Guid.Parse("2f7ef048-5b48-4b61-bb4f-69f89d152342") },
                new Genre { Id = Guid.NewGuid(), Name = "Action" },
                new Genre { Id = Guid.NewGuid(), Name = "FPS", ParentGenreId = Guid.Parse("58b38346-fd72-4cfc-84d6-d4d1e3cf09b7") },
                new Genre { Id = Guid.NewGuid(), Name = "TPS", ParentGenreId = Guid.Parse("58b38346-fd72-4cfc-84d6-d4d1e3cf09b7") },
                new Genre { Id = Guid.NewGuid(), Name = "Adventure" },
                new Genre { Id = Guid.NewGuid(), Name = "Puzzle & Skill" }
            );

            modelBuilder.Entity<Platform>().HasData(
                new Platform { Id = Guid.NewGuid(), Type = "Mobile" },
                new Platform { Id = Guid.NewGuid(), Type = "Browser" },
                new Platform { Id = Guid.NewGuid(), Type = "Desktop" },
                new Platform { Id = Guid.NewGuid(), Type = "Console" }
            );

            modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = Guid.NewGuid(),
                    Name = "Example Game",
                    Key = "example-game",
                    Description = "Example Description",
                    Price = 0,
                    UnitInStock = 0,
                    Discount = 0,
                    PublisherId = Guid.NewGuid()
                }
            );
        }
    }
}
