using System.Data.SQLite;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

internal static class MigrationRepairChecks
{
    public static void Run(Migration migration)
    {
        var sql = ((SqlOperation)migration.UpOperations.First()).Sql;
        var mapping = Regex.Matches(sql, @"\((\d+),(\d+),(\d+),(\d+)\)")
            .Select(m => m.Groups.Cast<Group>().Skip(1).Select(g => int.Parse(g.Value)).ToArray()).ToArray();
        Require(mapping.Length == 67, "Repair must contain all 67 fixed meter mappings.");

        foreach (var version in new[] { "original", "previous", "current", "missing" })
        {
            using var db = Fixture(mapping, version);
            ExecuteRepair(db, sql);
            Verify(db, mapping);
            // Running the repair against an already repaired database is harmless.
            ExecuteRepair(db, sql);
            Verify(db, mapping);
        }

        foreach (var conflict in new[] { "unknown number", "removed meter has data", "unrelated meter" })
        {
            using var db = Fixture(mapping, "original");
            Execute(db, conflict switch
            {
                "unknown number" => "UPDATE WaterMeters SET MapNumber = 9999 WHERE Id = 100010;",
                "removed meter has data" => "UPDATE WaterMeters SET Reading = '11111111' WHERE Id = 100052;",
                _ => "INSERT INTO WaterMeters VALUES (900001, 1, '12345678', '2026-11-09T12:00:00.0000000+02:00');"
            });
            string before = State(db);
            bool rejected = false;
            try { ExecuteRepair(db, sql); } catch (InvalidOperationException) { rejected = true; }
            Require(rejected && State(db) == before, $"Repair must preserve conflicting data: {conflict}.");
            Require((long)Scalar(db, "SELECT COUNT(*) FROM sqlite_master WHERE name = 'IX_WaterMeters_MapNumber'") == 1,
                "Failed repair removed the unique index.");
        }
        Console.WriteLine("PASS: actual repair DML handles city-wide, 1–68, current and incomplete meter seeds; repeated repairs preserve readings/dates; unknown mappings and non-prototype data remain untouched.");
    }

    private static SQLiteConnection Fixture(int[][] mapping, string version)
    {
        var db = new SQLiteConnection("Data Source=:memory:;");
        db.Open();
        Execute(db, "CREATE TABLE WaterMeters (Id INTEGER PRIMARY KEY, MapNumber int NOT NULL CHECK(MapNumber > 0), Reading char(8) NOT NULL, TakenAt text NOT NULL);");
        Execute(db, "CREATE UNIQUE INDEX IX_WaterMeters_MapNumber ON WaterMeters(MapNumber);");
        foreach (var row in mapping)
        {
            if (version == "missing" && row[0] == 100010) continue;
            int number = row[version == "current" ? 1 : version == "previous" ? 3 : 2];
            string reading = row[0] == 100057 ? "12345678" : "00000000";
            string date = row[0] == 100057 ? "2026-11-09T12:00:00.0000000+02:00" : "2026-11-08T22:00:00.0000000+02:00";
            Execute(db, $"INSERT INTO WaterMeters VALUES ({row[0]}, {number}, '{reading}', '{date}');");
        }
        if (version != "current")
            Execute(db, $"INSERT INTO WaterMeters VALUES (100052, {(version == "previous" ? 52 : 500)}, '00000000', '2026-11-08T22:00:00.0000000+02:00');");
        return db;
    }

    private static void Verify(SQLiteConnection db, int[][] mapping)
    {
        Require((long)Scalar(db, "SELECT COUNT(*) FROM WaterMeters") == 67, "Wrong repaired meter count.");
        foreach (var row in mapping)
            Require(Convert.ToInt64(Scalar(db, $"SELECT MapNumber FROM WaterMeters WHERE Id = {row[0]}")) == row[1], "Repaired mapping mismatch.");
        Require((string)Scalar(db, "SELECT Reading FROM WaterMeters WHERE Id = 100057") == "12345678", "Real reading was overwritten.");
        Require((string)Scalar(db, "SELECT TakenAt FROM WaterMeters WHERE Id = 100057") == "2026-11-09T12:00:00.0000000+02:00", "Reading date was overwritten.");
    }

    // Run the migration's actual SQL statements in an isolated relational database.
    // The adapter evaluates T-SQL IF/THROW guards, omits SQL Server IDENTITY_INSERT,
    // and adapts DROP INDEX syntax. Production validation still uses SQL Server SQL generation.
    private static void ExecuteRepair(SQLiteConnection db, string sql)
    {
        using var transaction = db.BeginTransaction();
        foreach (string part in Regex.Replace(sql, @"--[^\r\n]*", "").Split(';'))
        {
            string statement = part.Trim();
            if (statement.Length == 0 || statement.StartsWith("SET IDENTITY_INSERT")) continue;
            var guard = Regex.Match(statement, @"^IF EXISTS\s*\((.*)\)\s*THROW 51000, '([^']*)', 1$", RegexOptions.Singleline);
            if (guard.Success)
            {
                using var command = new SQLiteCommand(guard.Groups[1].Value, db, transaction);
                if (command.ExecuteScalar() != null) throw new InvalidOperationException(guard.Groups[2].Value);
                continue;
            }
            if (statement.StartsWith("IF ")) throw new Exception("Unrecognized SQL guard in test adapter.");
            if (statement.StartsWith("DROP INDEX")) statement = statement.Replace(" ON [WaterMeters]", "");
            using var mutation = new SQLiteCommand(statement, db, transaction);
            mutation.ExecuteNonQuery();
        }
        transaction.Commit();
    }

    private static string State(SQLiteConnection db)
    {
        using var command = new SQLiteCommand("SELECT Id, MapNumber, Reading, TakenAt FROM WaterMeters ORDER BY Id", db);
        using var reader = command.ExecuteReader();
        var rows = new List<string>();
        while (reader.Read()) rows.Add($"{reader[0]}|{reader[1]}|{reader[2]}|{reader[3]}");
        return string.Join("\n", rows);
    }

    private static void Execute(SQLiteConnection db, string sql)
    {
        using var command = new SQLiteCommand(sql, db);
        command.ExecuteNonQuery();
    }

    private static object Scalar(SQLiteConnection db, string sql)
    {
        using var command = new SQLiteCommand(sql, db);
        return command.ExecuteScalar();
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}
