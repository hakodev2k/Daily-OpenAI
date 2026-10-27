using GuidWireLab;
if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
{
    Console.Error.WriteLine("Usage: dotnet run --project checks/ContractChecks.csproj -- reproduce|verify");
    return 2;
}
Guid id = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff");
byte[] network = Convert.FromHexString("00112233445566778899AABBCCDDEEFF");
Guid id2 = Guid.Parse("12345678-9abc-def0-1122-334455667788");
byte[] network2 = Convert.FromHexString("123456789ABCDEF01122334455667788");
int failed = 0;
void Check(bool ok, string label)
{
    Console.WriteLine((ok ? "PASS " : "FAIL ") + label);
    if (!ok) failed++;
}
byte[] encoded = VehicleEventCodec.EncodeVehicleId(id);
Guid external = VehicleEventCodec.DecodeVehicleId(network);
Guid local = VehicleEventCodec.DecodeVehicleId(encoded);
if (args[0] == "reproduce")
{
    Console.WriteLine("Reproduce: PASS confirms the original defect, not a fixed system.");
    Check(local == id, "Internal round-trip succeeds");
    Check(!encoded.SequenceEqual(network), "Outbound bytes differ from partner golden vector");
    Check(external != id, "Inbound partner frame resolves to a different vehicle");
}
else
{
    Console.WriteLine("Verify: PASS validates the learner-edited starter code.");
    Check(encoded.SequenceEqual(network), "Encode partner golden vector A");
    Check(external == id, "Decode partner golden vector A");
    Check(local == id, "Internal round-trip remains valid");
    Check(VehicleEventCodec.EncodeVehicleId(id2).SequenceEqual(network2), "Encode golden vector B");
    Check(VehicleEventCodec.DecodeVehicleId(network2) == id2, "Decode golden vector B");
    bool rejected = false;
    try { VehicleEventCodec.DecodeVehicleId(network.AsSpan(0, 15)); }
    catch (ArgumentException) { rejected = true; }
    Check(rejected, "Reject truncated 15-byte frame");
}
Console.WriteLine($"Failed assertions: {failed}");
return failed == 0 ? 0 : 1;
