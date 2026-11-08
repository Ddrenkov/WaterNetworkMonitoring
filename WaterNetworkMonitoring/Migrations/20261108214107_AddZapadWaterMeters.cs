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
                    { 100001, 49, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100002, 50, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100003, 51, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100004, 194, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100005, 195, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100006, 196, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100007, 197, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100008, 198, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100009, 199, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100010, 200, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100011, 201, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100012, 202, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100013, 203, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100014, 204, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100015, 205, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100016, 206, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100017, 207, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100018, 208, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100019, 209, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100020, 210, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100021, 211, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100022, 212, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100023, 213, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100024, 214, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100025, 215, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100026, 216, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100027, 217, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100028, 218, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100029, 219, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100030, 220, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100031, 221, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100032, 222, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100033, 223, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100034, 224, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100035, 225, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100036, 226, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100037, 227, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100038, 228, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100039, 229, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100040, 230, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100041, 231, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100042, 232, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100043, 233, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100044, 234, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100045, 235, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100046, 236, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100047, 237, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100048, 238, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100049, 239, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100050, 240, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100051, 241, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100052, 500, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100053, 605, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100054, 606, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100055, 607, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100056, 608, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100057, 725, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100058, 1034, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100059, 2131, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100060, 2254, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100061, 2530, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100062, 2531, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100063, 2801, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100064, 3200, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100065, 3201, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100066, 3202, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100067, 3374, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) },
                    { 100068, 3375, "00000000", new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0)) }
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
                            MapNumber = 49,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100002,
                            MapNumber = 50,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100003,
                            MapNumber = 51,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100004,
                            MapNumber = 194,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100005,
                            MapNumber = 195,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100006,
                            MapNumber = 196,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100007,
                            MapNumber = 197,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100008,
                            MapNumber = 198,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100009,
                            MapNumber = 199,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100010,
                            MapNumber = 200,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100011,
                            MapNumber = 201,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100012,
                            MapNumber = 202,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100013,
                            MapNumber = 203,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100014,
                            MapNumber = 204,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100015,
                            MapNumber = 205,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100016,
                            MapNumber = 206,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100017,
                            MapNumber = 207,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100018,
                            MapNumber = 208,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100019,
                            MapNumber = 209,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100020,
                            MapNumber = 210,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100021,
                            MapNumber = 211,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100022,
                            MapNumber = 212,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100023,
                            MapNumber = 213,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100024,
                            MapNumber = 214,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100025,
                            MapNumber = 215,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100026,
                            MapNumber = 216,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100027,
                            MapNumber = 217,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100028,
                            MapNumber = 218,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100029,
                            MapNumber = 219,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100030,
                            MapNumber = 220,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100031,
                            MapNumber = 221,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100032,
                            MapNumber = 222,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100033,
                            MapNumber = 223,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100034,
                            MapNumber = 224,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100035,
                            MapNumber = 225,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100036,
                            MapNumber = 226,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100037,
                            MapNumber = 227,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100038,
                            MapNumber = 228,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100039,
                            MapNumber = 229,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100040,
                            MapNumber = 230,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100041,
                            MapNumber = 231,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100042,
                            MapNumber = 232,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100043,
                            MapNumber = 233,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100044,
                            MapNumber = 234,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100045,
                            MapNumber = 235,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100046,
                            MapNumber = 236,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100047,
                            MapNumber = 237,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100048,
                            MapNumber = 238,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100049,
                            MapNumber = 239,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100050,
                            MapNumber = 240,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100051,
                            MapNumber = 241,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100052,
                            MapNumber = 500,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100053,
                            MapNumber = 605,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100054,
                            MapNumber = 606,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100055,
                            MapNumber = 607,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100056,
                            MapNumber = 608,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100057,
                            MapNumber = 725,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100058,
                            MapNumber = 1034,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100059,
                            MapNumber = 2131,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100060,
                            MapNumber = 2254,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100061,
                            MapNumber = 2530,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100062,
                            MapNumber = 2531,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100063,
                            MapNumber = 2801,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100064,
                            MapNumber = 3200,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100065,
                            MapNumber = 3201,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100066,
                            MapNumber = 3202,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100067,
                            MapNumber = 3374,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        },
                        new
                        {
                            Id = 100068,
                            MapNumber = 3375,
                            Reading = "00000000",
                            TakenAt = new DateTimeOffset(new DateTime(2026, 11, 8, 22, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 2, 0, 0, 0))
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
