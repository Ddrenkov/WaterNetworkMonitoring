using Microsoft.EntityFrameworkCore;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<WaterMeter> WaterMeters { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=DenislavPC\MSSQLSERVER01;
                  Database=WaterNetworkDB;
                  Trusted_Connection=True;
                  TrustServerCertificate=True;"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Email)
                .IsUnique();

            var meters = modelBuilder.Entity<WaterMeter>();
            meters.ToTable("WaterMeters", table =>
            {
                table.HasCheckConstraint("CK_WaterMeters_SixDigitId", "[Id] BETWEEN 100000 AND 999999");
                table.HasCheckConstraint("CK_WaterMeters_MapNumber", "[MapNumber] > 0");
                table.HasCheckConstraint("CK_WaterMeters_Reading",
                    "LEN([Reading]) = 8 AND [Reading] COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%'");
            });
            meters.Property(m => m.Id).UseIdentityColumn(100001, 1);
            meters.HasIndex(m => m.MapNumber).IsUnique();
            meters.Property(m => m.Reading).HasMaxLength(8).IsFixedLength().IsUnicode(false).IsRequired();
            meters.HasData(ZapadWaterMeterSeed.GetReadings());
        }
    }
}
