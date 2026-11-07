namespace ParcelLab;
public sealed class Parcel
{
    public Parcel(int parcelId, int routeZone)
    {
        if (parcelId <= 0 || routeZone < 0) throw new ArgumentOutOfRangeException();
        ParcelId = parcelId;
        RouteZone = routeZone;
    }
    public int ParcelId { get; }
    public int RouteZone { get; private set; }
    public void ReassignRoute(int routeZone)
    {
        if (routeZone < 0) throw new ArgumentOutOfRangeException(nameof(routeZone));
        RouteZone = routeZone;
    }
}
