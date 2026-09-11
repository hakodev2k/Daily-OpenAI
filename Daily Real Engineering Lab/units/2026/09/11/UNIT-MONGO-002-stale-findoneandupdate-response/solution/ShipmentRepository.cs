using MongoDB.Bson;
using MongoDB.Driver;

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
        var options = new FindOneAndUpdateOptions<Shipment>
        {
            ReturnDocument = ReturnDocument.After
        };

        return _shipments.FindOneAndUpdateAsync(filter, update, options);
    }
}
