using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using WaterNetworkMonitoring.Data;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WaterNetworkMonitoring.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20261108214107_AddZapadWaterMeters")]
    public partial class AddZapadWaterMeters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WaterMeters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "100001, 1"),
                    MapNumber = table.Column<int>(type: "int", nullable: false),
                    Reading = table.Column<string>(type: "char(8)", unicode: false, fixedLength: true, maxLength: 8, nullable: false),
                    TakenAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterMeters", x => x.Id);
                    table.CheckConstraint("CK_WaterMeters_MapNumber", "[MapNumber] > 0");
                    table.CheckConstraint("CK_WaterMeters_Reading", "LEN([Reading]) = 8 AND [Reading] COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%'");
                    table.CheckConstraint("CK_WaterMeters_SixDigitId", "[Id] BETWEEN 100000 AND 999999");
                });

            migrationBuilder.InsertData(
                table: "WaterMeters",
                columns: new[] { "Id", "MapNumber", "Reading", "TakenAt" },
                values: new object[,]
                {
                    { 100001, 1, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100002, 2, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100003, 3, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100004, 4, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100005, 5, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100006, 6, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100007, 7, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100008, 8, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100009, 9, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100010, 10, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100011, 11, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100012, 12, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100013, 13, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100014, 14, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100015, 15, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100016, 16, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100017, 17, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100018, 18, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100019, 19, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100020, 20, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100021, 21, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100022, 22, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100023, 23, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100024, 24, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100025, 25, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100026, 26, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100027, 27, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100028, 28, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100029, 29, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100030, 30, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100031, 31, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100032, 32, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100033, 33, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100034, 34, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100035, 35, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100036, 36, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100037, 37, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100038, 38, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100039, 39, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100040, 40, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100041, 41, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100042, 42, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100043, 43, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100044, 44, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100045, 45, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100046, 46, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100047, 47, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100048, 48, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100049, 49, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100050, 50, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100051, 51, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100053, 52, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100054, 53, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100055, 54, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100056, 55, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100057, 56, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100058, 57, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100059, 58, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100060, 59, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100061, 60, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100062, 61, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100063, 62, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100064, 63, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100065, 64, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100066, 65, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100067, 66, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100068, 67, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaterMeters_MapNumber",
                table: "WaterMeters",
                column: "MapNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaterMeters");
        }

        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.26")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WaterNetworkMonitoring.Models.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("WaterNetworkMonitoring.Models.WaterMeter", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 100001L);

                    b.Property<int>("MapNumber")
                        .HasColumnType("int");

                    b.Property<string>("Reading")
                        .IsRequired()
                        .HasMaxLength(8)
                        .IsUnicode(false)
                        .HasColumnType("char(8)")
                        .IsFixedLength();

                    b.Property<DateTimeOffset>("TakenAt")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("MapNumber")
                        .IsUnique();

                    b.ToTable("WaterMeters", null, t =>
                        {
                            t.HasCheckConstraint("CK_WaterMeters_MapNumber", "[MapNumber] > 0");

                            t.HasCheckConstraint("CK_WaterMeters_Reading", "LEN([Reading]) = 8 AND [Reading] COLLATE Latin1_General_100_BIN2 NOT LIKE '%[^0-9]%'");

                            t.HasCheckConstraint("CK_WaterMeters_SixDigitId", "[Id] BETWEEN 100000 AND 999999");
                        });

                    b.HasData(
                        new
                        {
                            Id = 100001,
                            MapNumber = 1,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100002,
                            MapNumber = 2,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100003,
                            MapNumber = 3,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100004,
                            MapNumber = 4,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100005,
                            MapNumber = 5,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100006,
                            MapNumber = 6,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100007,
                            MapNumber = 7,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100008,
                            MapNumber = 8,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100009,
                            MapNumber = 9,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100010,
                            MapNumber = 10,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100011,
                            MapNumber = 11,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100012,
                            MapNumber = 12,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100013,
                            MapNumber = 13,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100014,
                            MapNumber = 14,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100015,
                            MapNumber = 15,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100016,
                            MapNumber = 16,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100017,
                            MapNumber = 17,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100018,
                            MapNumber = 18,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100019,
                            MapNumber = 19,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100020,
                            MapNumber = 20,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100021,
                            MapNumber = 21,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100022,
                            MapNumber = 22,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100023,
                            MapNumber = 23,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100024,
                            MapNumber = 24,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100025,
                            MapNumber = 25,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100026,
                            MapNumber = 26,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100027,
                            MapNumber = 27,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100028,
                            MapNumber = 28,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100029,
                            MapNumber = 29,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100030,
                            MapNumber = 30,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100031,
                            MapNumber = 31,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100032,
                            MapNumber = 32,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100033,
                            MapNumber = 33,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100034,
                            MapNumber = 34,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100035,
                            MapNumber = 35,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100036,
                            MapNumber = 36,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100037,
                            MapNumber = 37,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100038,
                            MapNumber = 38,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100039,
                            MapNumber = 39,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100040,
                            MapNumber = 40,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100041,
                            MapNumber = 41,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100042,
                            MapNumber = 42,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100043,
                            MapNumber = 43,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100044,
                            MapNumber = 44,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100045,
                            MapNumber = 45,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100046,
                            MapNumber = 46,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100047,
                            MapNumber = 47,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100048,
                            MapNumber = 48,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100049,
                            MapNumber = 49,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100050,
                            MapNumber = 50,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100051,
                            MapNumber = 51,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100053,
                            MapNumber = 52,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100054,
                            MapNumber = 53,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100055,
                            MapNumber = 54,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100056,
                            MapNumber = 55,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100057,
                            MapNumber = 56,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100058,
                            MapNumber = 57,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100059,
                            MapNumber = 58,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100060,
                            MapNumber = 59,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100061,
                            MapNumber = 60,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100062,
                            MapNumber = 61,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100063,
                            MapNumber = 62,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100064,
                            MapNumber = 63,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100065,
                            MapNumber = 64,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100066,
                            MapNumber = 65,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100067,
                            MapNumber = 66,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100068,
                            MapNumber = 67,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
