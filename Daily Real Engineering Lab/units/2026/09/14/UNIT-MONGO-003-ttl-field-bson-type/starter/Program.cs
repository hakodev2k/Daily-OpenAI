using MongoDB.Bson;

var mode = args.FirstOrDefault() ?? "reproduce";
var expiresAt = DateTime.UtcNow.AddMinutes(-30);

var session = new SessionDocument
{
    Id = "session-42",
    UserId = "user-7",
    ExpiresAt = expiresAt.ToString("O")
};

var bson = session.ToBsonDocument();
var value = bson[nameof(SessionDocument.ExpiresAt)];

Console.WriteLine($"ExpiresAt value: {value}");
Console.WriteLine($"ExpiresAt BSON type: {value.BsonType}");

var ttlContractIsCorrect = value.BsonType == BsonType.DateTime;

if (mode.Equals("check", StringComparison.OrdinalIgnoreCase))
{
    if (!ttlContractIsCorrect)
    {
        Console.Error.WriteLine("CHECK FAILED: ExpiresAt is not stored as BSON DateTime.");
        Environment.ExitCode = 1;
        return;
    }

    Console.WriteLine("CHECK PASSED: ExpiresAt is stored as BSON DateTime.");
    return;
}

if (ttlContractIsCorrect)
{
    Console.Error.WriteLine("REPRODUCE FAILED: starter no longer demonstrates the intended storage contract problem.");
    Environment.ExitCode = 1;
    return;
}

Console.WriteLine("REPRODUCE PASSED: the expiration field looks timestamp-like but is stored with a different BSON type.");

sealed class SessionDocument
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ExpiresAt { get; set; } = string.Empty;
}
