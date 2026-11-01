using Microsoft.Data.Sqlite;

namespace PumpSyncLab;

internal static class Program
{
    private static int Main(string[] args)
    {
        string mode = args.Length == 0 ? "demo" : args[0];
        if (args.Length > 1 || mode is not ("demo" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Mode: demo | reproduce | verify");
            return 2;
        }

        using var db = new SqliteConnection("Data Source=:memory:");
        db.Open();
        Execute(db, "PRAGMA foreign_keys = ON;");
        Execute(db, """
            CREATE TABLE Equipment (
                Id TEXT PRIMARY KEY,
                Label TEXT NOT NULL
            );
            """);
        Execute(db, """
            CREATE TABLE Inspections (
                Id TEXT PRIMARY KEY,
                EquipmentId TEXT NOT NULL
                    REFERENCES Equipment(Id) ON DELETE CASCADE,
                Verdict TEXT NOT NULL
            );
            """);
        Execute(db, "INSERT INTO Equipment (Id, Label) VALUES ('P-017', 'Bơm số 17 - bản gốc');");
        Execute(db, """
            INSERT INTO Inspections (Id, EquipmentId, Verdict)
            VALUES ('I-001', 'P-017', 'PASS'),
                   ('I-002', 'P-017', 'FOLLOW_UP');
            """);

        long before = Count(db, "SELECT COUNT(*) FROM Inspections WHERE EquipmentId = $id;", "P-017");
        EquipmentImporter.Apply(db, "P-017", "Bơm số 17 - cập nhật lần 1");
        long afterFirst = Count(db, "SELECT COUNT(*) FROM Inspections WHERE EquipmentId = $id;", "P-017");

        EquipmentImporter.Apply(db, "P-099", "Bơm số 99 - mới");
        long inserted = Count(db, "SELECT COUNT(*) FROM Equipment WHERE Id = $id;", "P-099");

        EquipmentImporter.Apply(db, "P-017", "Bơm số 17 - cập nhật lần 2");
        long afterSecond = Count(db, "SELECT COUNT(*) FROM Inspections WHERE EquipmentId = $id;", "P-017");
        string? finalLabel = GetLabel(db, "P-017");
        int violations = ForeignKeyViolations(db);

        Console.WriteLine($"Inspections before sync: {before}");
        Console.WriteLine($"Inspections after first sync: {afterFirst}");
        Console.WriteLine($"Inspections after second sync: {afterSecond}");
        Console.WriteLine($"New equipment rows: {inserted}");
        Console.WriteLine($"Final label: {finalLabel}");
        Console.WriteLine($"Foreign key violations: {violations}");

        bool common = before == 2 && inserted == 1
            && finalLabel == "Bơm số 17 - cập nhật lần 2" && violations == 0;
        bool symptom = common && afterFirst == 0 && afterSecond == 0;
        bool correct = common && afterFirst == 2 && afterSecond == 2;

        if (mode == "reproduce")
        {
            Console.WriteLine(symptom
                ? "REPRODUCED: inspection history vanished after sync."
                : "NOT REPRODUCED: original symptom absent.");
            return symptom ? 0 : 1;
        }
        if (mode == "verify")
        {
            Console.WriteLine(correct
                ? "PASS: metadata updated, new equipment inserted, inspection history preserved."
                : "FAIL: synchronization violates history-preservation contract.");
            return correct ? 0 : 1;
        }
        Console.WriteLine("DEMO ONLY: reproduce before editing; verify after editing.");
        return 0;
    }

    private static void Execute(SqliteConnection db, string sql)
    {
        using var command = db.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static long Count(SqliteConnection db, string sql, string id)
    {
        using var command = db.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("$id", id);
        return (long)command.ExecuteScalar()!;
    }

    private static string? GetLabel(SqliteConnection db, string id)
    {
        using var command = db.CreateCommand();
        command.CommandText = "SELECT Label FROM Equipment WHERE Id = $id;";
        command.Parameters.AddWithValue("$id", id);
        return command.ExecuteScalar() as string;
    }

    private static int ForeignKeyViolations(SqliteConnection db)
    {
        using var command = db.CreateCommand();
        command.CommandText = "PRAGMA foreign_key_check;";
        using var reader = command.ExecuteReader();
        int count = 0;
        while (reader.Read()) count++;
        return count;
    }
}
