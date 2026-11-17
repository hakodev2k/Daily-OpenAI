// Mô phỏng deployment trong 8 giây: khởi động process tại t=0, nạp lịch khám xong tại t=4.
var startupCompletedAt = 4;
var firstTrafficAt = -1;
var failures = 0;
for (var second = 0; second < 8; second++)
{
    var processAlive = true;
    var appointmentsReady = second >= startupCompletedAt;
    var liveness = processAlive;
    var readiness = processAlive; // Quy tắc hiện tại do nhóm vận hành cấu hình.
    var routed = readiness;
    var status = routed ? (appointmentsReady ? 200 : 503) : 0;
    if (routed && firstTrafficAt < 0) firstTrafficAt = second;
    if (status == 503) failures++;
    Console.WriteLine($"t={second} live={liveness} ready={readiness} routed={routed} http={status}");
}
Console.WriteLine($"firstTraffic={firstTrafficAt} failures={failures}");
if (args.Contains("--verify"))
{
    if (firstTrafficAt != startupCompletedAt || failures != 0)
    {
        Console.Error.WriteLine("VERIFY_FAIL: traffic reached an instance outside its service-ready window");
        return 1;
    }
    Console.WriteLine("VERIFY_PASS");
}
else if (failures == 0)
{
    Console.Error.WriteLine("REPRO_FAIL: expected starter symptom absent");
    return 2;
}
return 0;
