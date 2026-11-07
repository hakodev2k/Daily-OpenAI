using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

var client = new MongoClient("mongodb://localhost:27027");
var database = client.GetDatabase("engineering_lab");
var profiles = database.GetCollection<Profile>("profiles");

await profiles.DeleteManyAsync(FilterDefinition<Profile>.Empty);

var profile = new Profile
{
    Id = ObjectId.GenerateNewId(),
    DisplayName = "Ha"
};

await profiles.InsertOneAsync(profile);
Console.WriteLine($"SEEDED id={profile.Id} displayName={profile.DisplayName}");

await SimulateApiUpdate(profile.Id, "Ha Nguyen");
await SimulateApiUpdate(profile.Id, "Ha Nguyen");
await SimulateApiUpdate(ObjectId.GenerateNewId(), "Nobody");

async Task SimulateApiUpdate(ObjectId id, string newDisplayName)
{
    var filter = Builders<Profile>.Filter.Eq(x => x.Id, id);
    var update = Builders<Profile>.Update.Set(x => x.DisplayName, newDisplayName);

    var result = await profiles.UpdateOneAsync(filter, update);

    // Investigation note:
    // Which result field answers "did a document exist?"
    // and which field answers "did persisted data actually change?"
    var apiStatus = result.ModifiedCount == 0 ? "NOT_FOUND" : "OK";

    Console.WriteLine(
        $"UPDATE id={id} value={newDisplayName} matched={result.MatchedCount} modified={result.ModifiedCount} api={apiStatus}");
}

public sealed class Profile
{
    [BsonId]
    public ObjectId Id { get; set; }

    public string DisplayName { get; set; } = string.Empty;
}
