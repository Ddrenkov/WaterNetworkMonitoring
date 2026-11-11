using Microsoft.EntityFrameworkCore;
using WaterNetworkMonitoring.Models;

namespace WaterNetworkMonitoring.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<WaterMeter> WaterMeters { get; set; }
        public DbSet<PressureLogger> Loggers { get; set; }
        public DbSet<LoggerReading> LoggerReadings { get; set; }
        public DbSet<WaterMeterReading> WaterMeterReadings { get; set; }

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("WATERNETWORK_CONNECTION_STRING") ??
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

            var loggers = modelBuilder.Entity<PressureLogger>();
            loggers.ToTable("Loggers", table =>
                table.HasCheckConstraint("CK_Loggers_SixDigitId", "[Id] BETWEEN 100000 AND 999999"));
            loggers.Property(l => l.Id).ValueGeneratedNever();
            loggers.Property(l => l.Name).HasMaxLength(100).IsRequired();
            loggers.Property(l => l.ZoneName).HasMaxLength(50).IsRequired();
            loggers.HasOne(l => l.ParentLogger).WithMany().HasForeignKey(l => l.ParentLoggerId)
                .OnDelete(DeleteBehavior.Restrict);
            loggers.HasOne(l => l.SuppliedWaterMeter).WithMany().HasForeignKey(l => l.SuppliedWaterMeterId)
                .OnDelete(DeleteBehavior.Restrict);

            var loggerReadings = modelBuilder.Entity<LoggerReading>();
            loggerReadings.ToTable("LoggerReadings", table =>
            {
                table.HasCheckConstraint("CK_LoggerReadings_Flow", "[FlowM3PerHour] >= 0");
                table.HasCheckConstraint("CK_LoggerReadings_Pressure", "[PressureBar] >= 0");
            });
            loggerReadings.Property(r => r.FlowM3PerHour).HasPrecision(12, 3);
            loggerReadings.Property(r => r.PressureBar).HasPrecision(6, 3);
            loggerReadings.HasIndex(r => new { r.LoggerId, r.TakenAt }).IsUnique();
            loggerReadings.HasOne(r => r.Logger).WithMany().HasForeignKey(r => r.LoggerId)
                .OnDelete(DeleteBehavior.Restrict);

            var meterReadings = modelBuilder.Entity<WaterMeterReading>();
            meterReadings.ToTable("WaterMeterReadings", table =>
                table.HasCheckConstraint("CK_WaterMeterReadings_Reading",
                    "LEN([Reading]) = 8 AND [Reading] COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%'"));
            meterReadings.Property(r => r.Reading).HasMaxLength(8).IsFixedLength().IsUnicode(false).IsRequired();
            meterReadings.HasIndex(r => new { r.WaterMeterId, r.TakenAt }).IsUnique();
            meterReadings.HasOne(r => r.WaterMeter).WithMany().HasForeignKey(r => r.WaterMeterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
