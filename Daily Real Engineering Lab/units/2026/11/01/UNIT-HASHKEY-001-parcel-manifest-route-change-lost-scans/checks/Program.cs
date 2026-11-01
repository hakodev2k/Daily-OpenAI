using ParcelLab;

string mode = args.Length == 1 ? args[0].ToLowerInvariant() : "observe";
if (mode != "observe" && mode != "reproduce" && mode != "verify")
{
    Console.Error.WriteLine("Expected observe, reproduce, or verify.");
    return 2;
}
var tests = new (string Name, bool Regression, Func<bool> Test)[]
{
    ("scan retained after route change", true, () => {
        var m = new Manifest(); var parcel = new Parcel(101, 2);
        m.RecordScan(parcel); parcel.ReassignRoute(9);
        return m.GetScanCount(parcel) == 1 && m.TrackedParcels == 1;
    }),
    ("second scan does not create phantom parcel", true, () => {
        var m = new Manifest(); var parcel = new Parcel(101, 2);
        m.RecordScan(parcel); parcel.ReassignRoute(9); m.RecordScan(parcel);
        return m.GetScanCount(parcel) == 2 && m.TrackedParcels == 1;
    }),
    ("same ID from another scanner", true, () => {
        var m = new Manifest();
        m.RecordScan(new Parcel(101, 2)); m.RecordScan(new Parcel(101, 9));
        return m.TrackedParcels == 1 && m.GetScanCount(new Parcel(101, 5)) == 2;
    }),
    ("repeated scans without route change", false, () => {
        var m = new Manifest(); var parcel = new Parcel(101, 2);
        m.RecordScan(parcel); m.RecordScan(parcel);
        return m.TrackedParcels == 1 && m.GetScanCount(parcel) == 2;
    }),
    ("distinct IDs remain distinct", false, () => {
        var m = new Manifest();
        m.RecordScan(new Parcel(101, 2)); m.RecordScan(new Parcel(102, 2));
        return m.TrackedParcels == 2 && m.GetScanCount(new Parcel(101, 2)) == 1
            && m.GetScanCount(new Parcel(102, 2)) == 1;
    })
};
int failedRegression = 0, failedBaseline = 0;
foreach (var test in tests)
{
    bool passed;
    try { passed = test.Test(); }
    catch (Exception ex)
    {
        passed = false;
        Console.WriteLine("EXCEPTION " + test.Name + ": " + ex.GetType().Name);
    }
    if (!passed)
    {
        if (test.Regression) failedRegression++; else failedBaseline++;
    }
    Console.WriteLine((passed ? "PASS | " : "FAIL | ") + test.Name);
}
Console.WriteLine("Regression failures=" + failedRegression + "; baseline failures=" + failedBaseline);
if (mode == "observe") return 0;
if (mode == "reproduce")
{
    bool reproduced = failedRegression == 3 && failedBaseline == 0;
    Console.WriteLine(reproduced ? "REPRODUCED: 3 expected contract failures" : "NOT REPRODUCED");
    return reproduced ? 0 : 1;
}
bool verified = failedRegression == 0 && failedBaseline == 0;
Console.WriteLine(verified ? "VERIFIED: all 5 behavior contracts pass" : "NOT VERIFIED");
return verified ? 0 : 1;
