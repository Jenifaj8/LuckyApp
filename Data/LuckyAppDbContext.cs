using LuckyApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LuckyApp.Data
{
    public class LuckyAppDbContext : DbContext
    {
        public LuckyAppDbContext(
            DbContextOptions<LuckyAppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pick4Draw> Pick4Draws { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Pick4Draw>(entity =>
            {
                entity.HasKey(draw => draw.Id);

                // Prevent duplicate draws.
                entity.HasIndex(draw => new
                    {
                        draw.DrawDate,
                        draw.DrawType
                    })
                    .IsUnique();

                entity.Property(draw => draw.DrawType)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(draw => draw.WinningNumber)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.Property(draw => draw.Source)
                    .HasMaxLength(500);

                entity.ToTable("Pick4Draws", table =>
                {
                    table.HasCheckConstraint(
                        "CK_Pick4Draw_DrawType",
                        "\"DrawType\" IN ('Midday', 'Night')");

                    table.HasCheckConstraint(
                        "CK_Pick4Draw_WinningNumber",
                        "\"WinningNumber\" ~ '^[0-9]{4}$'");
                });
            });
        }
    }
}