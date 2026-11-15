using System.Globalization;
using Microsoft.EntityFrameworkCore.Migrations;
using WaterNetworkMonitoring.Data;
using WaterNetworkMonitoring.Models;

#nullable disable

namespace WaterNetworkMonitoring.Migrations
{
    public partial class AddFourZapadLoggers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM Loggers WHERE Id BETWEEN 200001 AND 200004 AND (IsSimulated=0 OR ZoneName<>'Zapad'))
                    OR EXISTS (SELECT 1 FROM LoggerReadings WHERE LoggerId BETWEEN 200001 AND 200004 AND IsSimulated=0)
                    THROW 51001, 'The four-logger prototype must not overwrite real logger data.', 1;
                """);
            migrationBuilder.AddColumn<int>("LoggerId","WaterMeters",type:"int",nullable:true);
            migrationBuilder.AddColumn<double>("Latitude","Loggers",type:"float",nullable:false,defaultValue:0d);
            migrationBuilder.AddColumn<double>("Longitude","Loggers",type:"float",nullable:false,defaultValue:0d);
            foreach(var logger in ZapadFourLoggerSeed.GetLoggers())
                migrationBuilder.Sql($"""
                    MERGE Loggers AS target
                    USING (VALUES ({logger.Id},N'{logger.Name}',{Number(logger.Latitude)},{Number(logger.Longitude)},
                        {logger.ParentLoggerId?.ToString() ?? "NULL"},{logger.SuppliedWaterMeterId?.ToString() ?? "NULL"}))
                        AS source(Id,Name,Latitude,Longitude,ParentLoggerId,SuppliedWaterMeterId)
                    ON target.Id=source.Id
                    WHEN MATCHED THEN UPDATE SET Name=source.Name,Latitude=source.Latitude,Longitude=source.Longitude,
                        ParentLoggerId=source.ParentLoggerId,SuppliedWaterMeterId=source.SuppliedWaterMeterId
                    WHEN NOT MATCHED THEN INSERT (Id,Name,ZoneName,Latitude,Longitude,ParentLoggerId,SuppliedWaterMeterId,IsSimulated)
                        VALUES(source.Id,source.Name,N'Zapad',source.Latitude,source.Longitude,source.ParentLoggerId,source.SuppliedWaterMeterId,1);
                    """);
            foreach(var meter in ZapadWaterMeterSeed.GetReadings())
                migrationBuilder.UpdateData("WaterMeters","Id",meter.Id,"LoggerId",ZapadFourLoggerSeed.LoggerForMeter(meter.Id));
            migrationBuilder.CreateIndex("IX_WaterMeters_LoggerId","WaterMeters","LoggerId");
            migrationBuilder.AddForeignKey("FK_WaterMeters_Loggers_LoggerId","WaterMeters","LoggerId","Loggers",
                principalColumn:"Id",onDelete:ReferentialAction.Restrict);
            WriteReadings(migrationBuilder,ZapadFourLoggerSeed.GenerateReadings());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("FK_WaterMeters_Loggers_LoggerId","WaterMeters");
            migrationBuilder.DropIndex("IX_WaterMeters_LoggerId","WaterMeters");
            migrationBuilder.DropColumn("LoggerId","WaterMeters");
            WriteReadings(migrationBuilder,ZapadOctober2026Seed.Generate().LoggerReadings);
            migrationBuilder.Sql("DELETE FROM LoggerReadings WHERE LoggerId IN (200003,200004) AND IsSimulated=1;");
            migrationBuilder.Sql("DELETE FROM Loggers WHERE Id IN (200003,200004) AND IsSimulated=1;");
            migrationBuilder.DropColumn("Latitude","Loggers");
            migrationBuilder.DropColumn("Longitude","Loggers");
        }

        private static string Number(double value)=>value.ToString("R",CultureInfo.InvariantCulture);
        private static void WriteReadings(MigrationBuilder migrationBuilder,IEnumerable<LoggerReading> readings)
        {
            foreach(var batch in readings.Chunk(400))
            {
                string values=string.Join(",\n",batch.Select(r=>$"({r.LoggerId},CONVERT(datetime2(2),'{r.TakenAt.UtcDateTime:yyyy-MM-ddTHH:mm:ss.ff}',126),{r.FlowM3PerHour.ToString(CultureInfo.InvariantCulture)},{r.PressureBar.ToString(CultureInfo.InvariantCulture)})"));
                migrationBuilder.Sql($"""
                    MERGE LoggerReadings AS target
                    USING (VALUES {values}) AS source(LoggerId,TakenAtUtc,Flow,Pressure)
                    ON target.LoggerId=source.LoggerId AND target.TakenAtUtc=source.TakenAtUtc
                    WHEN MATCHED AND target.IsSimulated=1 THEN UPDATE SET FlowM3PerHour=source.Flow,PressureBar=source.Pressure
                    WHEN NOT MATCHED THEN INSERT (LoggerId,TakenAtUtc,FlowM3PerHour,PressureBar,IsSimulated)
                        VALUES(source.LoggerId,source.TakenAtUtc,source.Flow,source.Pressure,1);
                    """);
            }
        }
    }
}
