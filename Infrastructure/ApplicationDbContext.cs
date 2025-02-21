using Domain.Entites;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Article> Articles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);

            SeedRoleData(modelBuilder);

            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToTable("Article");
                entity.HasKey(e => e.ArticleId).HasName("article_id");

                entity.Property(e => e.ArticleId).HasColumnName("article_id");
                entity.Property(e => e.Content)
                    .HasColumnType("text")
                    .HasColumnName("content")
                    .IsRequired();

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Title)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("title")
                    .IsRequired();

                entity.HasOne(a => a.User)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }


        private void SeedRoleData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole<int>>().HasData(
                new IdentityRole<int>
                {
                    Id = 1,
                    Name = Enum.GetName(RoleType.Admin),
                    NormalizedName = Enum.GetName(RoleType.Admin).ToUpper()
                },
                new IdentityRole<int>
                {
                    Id = 2,
                    Name = Enum.GetName(RoleType.User),
                    NormalizedName = Enum.GetName(RoleType.User).ToUpper()
                }
            );
        }

    }
}