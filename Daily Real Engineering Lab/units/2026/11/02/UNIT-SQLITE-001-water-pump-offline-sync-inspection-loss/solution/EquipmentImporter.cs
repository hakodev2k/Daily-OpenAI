using Microsoft.Data.Sqlite;

namespace PumpSyncLab;

internal static class EquipmentImporter
{
    public static void Apply(SqliteConnection db, string equipmentId, string label)
    {
        using var command = db.CreateCommand();
        command.CommandText = """
            INSERT INTO Equipment (Id, Label)
            VALUES ($id, $label)
            ON CONFLICT(Id) DO UPDATE SET Label = excluded.Label;
            """;
        command.Parameters.AddWithValue("$id", equipmentId);
        command.Parameters.AddWithValue("$label", label);
        command.ExecuteNonQuery();
    }
}
