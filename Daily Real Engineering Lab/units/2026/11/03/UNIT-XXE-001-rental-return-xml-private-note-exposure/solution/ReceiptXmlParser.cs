using System.Xml;
using System.Xml.Linq;

namespace ReturnXmlLab;

internal sealed record ReturnReceipt(string ContractId, string Note);

internal sealed class ReceiptXmlParser
{
    public ReturnReceipt Parse(string xml)
    {
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            MaxCharactersFromEntities = 0
        };

        using var input = new StringReader(xml);
        using var reader = XmlReader.Create(input, settings);
        var document = XDocument.Load(reader);
        var root = document.Root ?? throw new XmlException("Missing receipt root");
        if (root.Name != "ReturnReceipt")
            throw new XmlException("Unexpected receipt root");

        var contractId = (string?)root.Element("ContractId");
        var note = (string?)root.Element("Note");
        if (string.IsNullOrWhiteSpace(contractId) || note is null)
            throw new XmlException("Incomplete return receipt");

        return new ReturnReceipt(contractId, note);
    }
}
