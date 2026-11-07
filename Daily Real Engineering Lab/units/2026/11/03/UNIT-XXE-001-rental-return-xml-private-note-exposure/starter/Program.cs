using System.Text;
using System.Xml;

namespace ReturnXmlLab;

internal static class Program
{
    private const string Canary = "LAB-CANARY-NOT-A-REAL-SECRET";

    private static int Main(string[] args)
    {
        if (args.Length != 1 || (args[0] != "--reproduce" && args[0] != "--verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run --project starter -- --reproduce|--verify");
            return 2;
        }

        // Controlled local fixture only. Never point the harness at a real secret.
        var path = Path.Combine(Path.GetTempPath(), $"xml-return-lab-{Guid.NewGuid():N}.txt");
        File.WriteAllText(path, Canary, new UTF8Encoding(false));
        try
        {
            var parser = new ReceiptXmlParser();
            const string normal = "<ReturnReceipt><ContractId>RNT-042</ContractId><Note>Return accepted</Note></ReturnReceipt>";
            var valid = parser.Parse(normal);
            bool validReceipt = valid.ContractId == "RNT-042" && valid.Note == "Return accepted";

            var uri = new Uri(Path.GetFullPath(path)).AbsoluteUri;
            var unusual = $"""
                <?xml version="1.0"?>
                <!DOCTYPE ReturnReceipt [<!ENTITY local SYSTEM "{uri}">]>
                <ReturnReceipt><ContractId>RNT-042</ContractId><Note>&local;</Note></ReturnReceipt>
                """;

            bool expanded = false;
            bool rejected = false;
            try
            {
                var receipt = parser.Parse(unusual);
                expanded = receipt.Note.Contains(Canary, StringComparison.Ordinal);
            }
            catch (XmlException)
            {
                rejected = true;
            }

            Console.WriteLine($"VALID_RECEIPT={(validReceipt ? "PASS" : "FAIL")}");
            Console.WriteLine($"EXTERNAL_ENTITY={(expanded ? "EXPANDED" : rejected ? "REJECTED" : "NOT_EXPANDED")}");
            Console.WriteLine("Canary source: generated local test fixture only");

            return args[0] == "--reproduce"
                ? (validReceipt && expanded ? 0 : 1)
                : (validReceipt && rejected && !expanded ? 0 : 1);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
