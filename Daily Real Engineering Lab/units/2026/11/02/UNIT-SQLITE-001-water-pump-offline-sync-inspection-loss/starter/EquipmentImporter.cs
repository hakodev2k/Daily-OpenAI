using Microsoft.Data.Sqlite;

namespace PumpSyncLab;

internal static class EquipmentImporter
{
    public static void Apply(SqliteConnection db, string equipmentId, string label)
    {
        using var command = db.CreateCommand();
        command.CommandText = """
            INSERT OR REPLACE INTO Equipment (Id, Label)
            VALUES ($id, $label);
            """;
        command.Parameters.AddWithValue("$id", equipmentId);
        command.Parameters.AddWithValue("$label", label);
        command.ExecuteNonQuery();
    }
}
