using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var mode = args.Length == 0 ? "report" : args[0].ToLowerInvariant();
if (mode is not ("report" or "reproduce" or "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project starter/CohortLab.csproj -- [report|reproduce|verify]");
    return 2;
}

await using var connection = new SqliteConnection("Data Source=:memory:");
await connection.OpenAsync();
var options = new DbContextOptionsBuilder<LabDb>()
    .UseSqlite(connection)
    .Options;

await using (var seed = new LabDb(options))
{
    await seed.Database.EnsureCreatedAsync();
    seed.Cohorts.Add(new Cohort
    {
        Id = 7,
        Name = "Autumn onboarding",
        Enrollments =
        [
            new Enrollment { Id = 101, IsActive = true },
            new Enrollment { Id = 102, IsActive = false },
            new Enrollment { Id = 103, IsActive = true }
        ]
    });
    await seed.SaveChangesAsync();
}

int[] coldIds;
await using (var cold = new LabDb(options))
{
    coldIds = await CohortReader.GetActiveEnrollmentIdsAsync(cold, 7);
}

int[] warmIds;
await using (var warmed = new LabDb(options))
{
    var prior = await warmed.Enrollments
        .Where(e => e.CohortId == 7)
        .OrderBy(e => e.Id)
        .ToListAsync();
    Console.WriteLine($"prior query rows={prior.Count}");
    warmIds = await CohortReader.GetActiveEnrollmentIdsAsync(warmed, 7);
}

var expected = new[] { 101, 103 };
var coldCorrect = coldIds.SequenceEqual(expected);
var warmedCorrect = warmIds.SequenceEqual(expected);
Console.WriteLine($"cold   IDs=[{string.Join(",", coldIds)}] correct={coldCorrect}");
Console.WriteLine($"warmed IDs=[{string.Join(",", warmIds)}] correct={warmedCorrect}");

if (mode == "report") return 0;
if (mode == "reproduce")
{
    var reproduced = coldCorrect && !warmedCorrect && warmIds.Contains(102);
    Console.WriteLine(reproduced ? "REPRODUCED: history-dependent read anomaly" : "NOT REPRODUCED: check original starter and dependencies");
    return reproduced ? 0 : 1;
}

// Verify both the cold and warmed read paths on the learner-editable starter code.
var valid = coldCorrect && warmedCorrect;
if (valid)
{
    await using var missingContext = new LabDb(options);
    try
    {
        await CohortReader.GetActiveEnrollmentIdsAsync(missingContext, 999);
        valid = false;
        Console.Error.WriteLine("REGRESSION: missing cohort must throw InvalidOperationException");
    }
    catch (InvalidOperationException)
    {
        Console.WriteLine("missing-cohort contract: PASS");
    }
}
Console.WriteLine(valid ? "PASS: cold/warmed roster and missing-cohort contract" : "FAIL: roster or missing-cohort contract");
return valid ? 0 : 1;
