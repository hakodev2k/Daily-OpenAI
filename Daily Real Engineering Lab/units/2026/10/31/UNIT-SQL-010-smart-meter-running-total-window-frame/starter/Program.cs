using Microsoft.Data.Sqlite;

var mode = args.Length == 0 ? "--run" : args[0];
if (mode is not ("--run" or "--reproduce" or "--verify"))
{
    Console.Error.WriteLine("Mode hợp lệ: --run | --reproduce | --verify");
    return 2;
}

using var db = new SqliteConnection("Data Source=:memory:");
db.Open();
using (var setup = db.CreateCommand())
{
    setup.CommandText = """
        CREATE TABLE meter_readings (
            reading_id INTEGER PRIMARY KEY,
            recorded_at TEXT NOT NULL,
            delta_wh INTEGER NOT NULL
        );
        INSERT INTO meter_readings VALUES (1, '2026-10-31T08:00:00', 100);
        INSERT INTO meter_readings VALUES (2, '2026-10-31T08:01:00', 20);
        INSERT INTO meter_readings VALUES (3, '2026-10-31T08:01:00', 30);
        INSERT INTO meter_readings VALUES (4, '2026-10-31T08:02:00', 40);
        """;
    setup.ExecuteNonQuery();
}
using var query = db.CreateCommand();
query.CommandText = """
    SELECT reading_id, recorded_at, delta_wh,
           SUM(delta_wh) OVER (ORDER BY recorded_at) AS cumulative_wh
    FROM meter_readings
    ORDER BY recorded_at, reading_id;
    """;

var rows = new List<(long Id, string At, long Delta, long Total)>();
using (var reader = query.ExecuteReader())
{
    while (reader.Read())
        rows.Add((reader.GetInt64(0), reader.GetString(1),
                  reader.GetInt64(2), reader.GetInt64(3)));
}
Console.WriteLine("Id | RecordedAt           | DeltaWh | CumulativeWh");
foreach (var row in rows)
    Console.WriteLine($"{row.Id,2} | {row.At} | {row.Delta,7} | {row.Total,12}");

long[] expected = [100, 120, 150, 190];
long[] observed = rows.Select(x => x.Total).ToArray();
bool valid = observed.SequenceEqual(expected);
if (mode == "--run")
{
    Console.WriteLine($"Business contract: {(valid ? "PASS" : "FAIL")}");
    return 0;
}
if (mode == "--reproduce")
{
    long[] originalSymptom = [100, 150, 150, 190];
    bool reproduced = observed.SequenceEqual(originalSymptom);
    Console.WriteLine(reproduced ? "REPRODUCED: original symptom observed." :
                                  "NOT REPRODUCED: original signature changed.");
    return reproduced ? 0 : 1;
}
Console.WriteLine(valid ? "VERIFY PASS: all four readings." :
                          "VERIFY FAIL: cumulative totals differ.");
return valid ? 0 : 1;
