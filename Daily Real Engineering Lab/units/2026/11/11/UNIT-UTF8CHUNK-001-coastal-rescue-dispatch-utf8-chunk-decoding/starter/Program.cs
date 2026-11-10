using System.Text;

const string message = "Đội cứu hộ: thuyền số 7 đã đến bến — ưu tiên trẻ em 🛟";
byte[] payload = Encoding.UTF8.GetBytes(message);
string mode = args.Length > 0 ? args[0] : "reproduce";
int[] sizes = [1, 2, 3, 4, 5, 7, 64];

if (mode == "reproduce")
{
    string observed = Decode(payload, 1);
    Console.WriteLine($"Input bytes: {payload.Length}; output: {observed}");
    Console.WriteLine($"Replacement chars: {observed.Count(c => c == '\uFFFD')}");
    if (observed == message)
    {
        Console.Error.WriteLine("Expected starter symptom was not reproduced.");
        return 1;
    }
    Console.WriteLine("REPRODUCED: fragmented delivery changes valid text.");
    return 0;
}
if (mode == "verify")
{
    foreach (int size in sizes)
    {
        string actual = Decode(payload, size);
        if (actual != message)
        {
            Console.Error.WriteLine($"FAIL: chunk={size}; expected={message}; actual={actual}");
            return 1;
        }
    }
    byte[] invalid = [0x41, 0xC3, 0x28, 0x42];
    foreach (int size in sizes)
    {
        string actual = Decode(invalid, size);
        string expected = Encoding.UTF8.GetString(invalid);
        if (actual != expected)
        {
            Console.Error.WriteLine($"FAIL invalid-input chunk={size}");
            return 1;
        }
    }
    Console.WriteLine("PASS: all fragmentation and malformed-input contracts.");
    return 0;
}
Console.Error.WriteLine("Usage: dotnet run --project starter/RescueDecoder.csproj -- reproduce|verify");
return 2;

static string Decode(byte[] bytes, int chunkSize)
{
    var output = new StringBuilder();
    for (int offset = 0; offset < bytes.Length; offset += chunkSize)
    {
        int count = Math.Min(chunkSize, bytes.Length - offset);
        output.Append(Encoding.UTF8.GetString(bytes, offset, count));
    }
    return output.ToString();
}
