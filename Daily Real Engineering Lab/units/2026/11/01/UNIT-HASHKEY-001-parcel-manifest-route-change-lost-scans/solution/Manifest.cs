namespace ParcelLab;
public sealed class Manifest
{
    private readonly Dictionary<Parcel, int> _counts = new(new ParcelComparer());
    public int TrackedParcels => _counts.Count;
    public void RecordScan(Parcel parcel)
    {
        ArgumentNullException.ThrowIfNull(parcel);
        _counts.TryGetValue(parcel, out int prior);
        _counts[parcel] = checked(prior + 1);
    }
    public int GetScanCount(Parcel parcel)
    {
        ArgumentNullException.ThrowIfNull(parcel);
        return _counts.TryGetValue(parcel, out int count) ? count : 0;
    }
    private sealed class ParcelComparer : IEqualityComparer<Parcel>
    {
        public bool Equals(Parcel? x, Parcel? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.ParcelId == y.ParcelId;
        }
        public int GetHashCode(Parcel obj)
        {
            ArgumentNullException.ThrowIfNull(obj);
            return obj.ParcelId;
        }
    }
}
