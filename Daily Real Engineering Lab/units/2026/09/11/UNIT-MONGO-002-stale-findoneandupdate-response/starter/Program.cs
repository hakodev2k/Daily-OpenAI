using MongoDB.Bson;
using MongoDB.Driver;

var mode = args.FirstOrDefault() ?? "reproduce";
var client = new MongoClient("mongodb://localhost:27018");
var database = client.GetDatabase("real_engineering_lab");
var collection = database.GetCollection<Shipment>("shipments");

await collection.DeleteManyAsync(FilterDefinition<Shipment>.Empty);
var shipment = new Shipment
{
    Id = ObjectId.GenerateNewId(),
    Reference = "SHIP-1001",
    Status = "Pending"
};
await collection.InsertOneAsync(shipment);

var before = await collection.Find(x => x.Id == shipment.Id).SingleAsync();
var repository = new ShipmentRepository(collection);
var returned = await repository.MarkReadyAsync(shipment.Id);
var stored = await collection.Find(x => x.Id == shipment.Id).SingleAsync();

Console.WriteLine($"BEFORE_STATUS={before.Status}");
Console.WriteLine($"RETURNED_STATUS={returned?.Status ?? "<null>"}");
Console.WriteLine($"STORED_STATUS={stored.Status}");

if (mode.Equals("reproduce", StringComparison.OrdinalIgnoreCase))
{
    return before.Status == "Pending" && returned?.Status == "Pending" && stored.Status == "Ready"
        ? 0
        : 2;
}

if (mode.Equals("verify", StringComparison.OrdinalIgnoreCase))
{
    return before.Status == "Pending" && returned?.Status == "Ready" && stored.Status == "Ready"
        ? 0
        : 3;
}

Console.Error.WriteLine("Unknown mode. Use reproduce or verify.");
return 4;

public sealed class Shipment
{
    public ObjectId Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public sealed class ShipmentRepository
{
    private readonly IMongoCollection<Shipment> _shipments;

    public ShipmentRepository(IMongoCollection<Shipment> shipments)
    {
        _shipments = shipments;
    }

    public Task<Shipment?> MarkReadyAsync(ObjectId id)
    {
        var filter = Builders<Shipment>.Filter.Eq(x => x.Id, id);
        var update = Builders<Shipment>.Update.Set(x => x.Status, "Ready");

        // Investigation note:
        // The database write succeeds. What exactly does this operation promise to return?
        return _shipments.FindOneAndUpdateAsync(filter, update);
    }
}
