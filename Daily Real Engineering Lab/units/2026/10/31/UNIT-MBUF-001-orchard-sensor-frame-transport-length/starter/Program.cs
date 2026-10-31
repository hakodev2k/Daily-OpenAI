var mode = args.Length == 0 ? "--reproduce" : args[0];
if (mode is not ("--reproduce" or "--verify"))
{
    Console.Error.WriteLine("Usage: --reproduce | --verify");
    return 2;
}

var packets = new[]
{
    new SensorPacket("ORCH-A11", -4, 128),
    new SensorPacket("ORCH-B02", 2, 256),
    new SensorPacket("ORCH-C19", 11, 512)
};

var exporter = new FrameExporter();
var receiver = new FrameReceiver();
var accepted = 0;
var rejected = 0;

foreach (var packet in packets)
{
    var upload = exporter.Build(packet);
    var ok = receiver.TryAccept(upload, packet, out var reason);
    if (ok) accepted++; else rejected++;

    Console.WriteLine(
        $"CASE {packet.SensorId}: {(ok ? "ACCEPT" : "REJECT")}, " +
        $"sentBytes={upload.Payload.Length}, declaredBytes={upload.DeclaredLength}, reason={reason}");
}

// A broken frame must remain rejected even after a successful learner fix.
var malformed = new UploadEnvelope(new byte[] { 0xA7, 0x00 }, 2);
var negativeRejected = !receiver.TryAccept(malformed, packets[0], out _);

if (mode == "--reproduce")
{
    var reproduced = rejected == packets.Length && accepted == 0 && negativeRejected;
    Console.WriteLine(reproduced ? "REPRODUCED: three valid frames rejected." :
        "NOT REPRODUCED: original failure contract not observed.");
    return reproduced ? 0 : 1;
}

var verified = accepted == packets.Length && rejected == 0 && negativeRejected;
Console.WriteLine(verified ? "VERIFIED: three valid frames accepted; malformed frame rejected." :
    "NOT VERIFIED: valid frames or negative case violate the transport contract.");
return verified ? 0 : 1;
