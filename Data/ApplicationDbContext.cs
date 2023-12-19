using Microsoft.EntityFrameworkCore;
using Ship_Review_API.Models;

namespace Ship_Review_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ShipInfo> ShipInfos { get; set; }

        public DbSet<ShipEvaluation> ShipEvaluations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            modelBuilder.Entity<ShipInfo>()
                .HasMany<ShipEvaluation>(s => s.ShipEvaluations)
                .WithOne(g => g.ShipInfos)
                .HasForeignKey(g => g.Imo)
                .OnDelete(DeleteBehavior.Cascade);
            

        }

    }
}
