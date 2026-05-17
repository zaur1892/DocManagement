using EvtapAz.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EvtapAz.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Settlement> Settlements { get; set; }
        public DbSet<MetroStation> MetroStations { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<BuildingType> BuildingTypes { get; set; }
        public DbSet<BuildingProject> BuildingProjects { get; set; }
        public DbSet<RepairType> RepairTypes { get; set; }
        public DbSet<Listing> Listings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Listing>()
                .Property(l => l.PriceMin).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Listing>()
                .Property(l => l.PriceMax).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Listing>()
                .Property(l => l.AreaMin).HasColumnType("decimal(10,2)");
            modelBuilder.Entity<Listing>()
                .Property(l => l.AreaMax).HasColumnType("decimal(10,2)");

            modelBuilder.Entity<Listing>()
                .HasOne(l => l.City).WithMany(c => c.Listings)
                .HasForeignKey(l => l.CityId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Listing>()
                .HasOne(l => l.District).WithMany(d => d.Listings)
                .HasForeignKey(l => l.DistrictId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Listing>()
                .HasOne(l => l.Settlement).WithMany(s => s.Listings)
                .HasForeignKey(l => l.SettlementId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
