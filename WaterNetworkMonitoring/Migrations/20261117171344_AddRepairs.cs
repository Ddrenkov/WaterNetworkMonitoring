using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WaterNetworkMonitoring.Migrations
{
    /// <inheritdoc />
    public partial class AddRepairs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Repairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2(2)", nullable: false),
                    Place = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WaterSystem = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repairs", x => x.Id);
                    table.CheckConstraint("CK_Repairs_WaterSystem", "[WaterSystem] IN ('CleanWater', 'Wastewater')");
                });

            migrationBuilder.InsertData(
                table: "Repairs",
                columns: new[] { "Id", "Date", "Place", "WaterSystem" },
                values: new object[] { 1, new DateTime(2026, 10, 31, 18, 0, 0, 0, DateTimeKind.Unspecified), "Zapad - Skaptopara supply branch", "CleanWater" });

            migrationBuilder.CreateIndex(
                name: "IX_Repairs_WaterSystem_Date",
                table: "Repairs",
                columns: new[] { "WaterSystem", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Repairs");
        }
    }
}
