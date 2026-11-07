// Reference: replace the CsvImporter class in starter/Program.cs with this class.
public static class CsvImporter
{
    public static List<NurseryBatch> Parse(string csv)
    {
        var records = new List<List<string>>();
        var fields = new List<string>();
        var field = new System.Text.StringBuilder();
        bool quoted = false, closedQuote = false, atFieldStart = true;
        for (int i = 0; i < csv.Length; i++)
        {
            char ch = csv[i];
            if (quoted)
            {
                if (ch == '"')
                {
                    if (i + 1 < csv.Length && csv[i + 1] == '"')
                    { field.Append('"'); i++; }
                    else { quoted = false; closedQuote = true; }
                }
                else field.Append(ch);
                continue;
            }
            if (ch == ',' || ch == '\r' || ch == '\n')
            {
                fields.Add(field.ToString()); field.Clear();
                atFieldStart = true; closedQuote = false;
                if (ch != ',')
                {
                    if (ch == '\r' && i + 1 < csv.Length && csv[i + 1] == '\n') i++;
                    records.Add(fields); fields = new List<string>();
                }
            }
            else if (ch == '"' && atFieldStart && !closedQuote)
            { quoted = true; atFieldStart = false; }
            else
            {
                if (ch == '"' || closedQuote) throw new FormatException("Malformed quoted CSV field");
                field.Append(ch); atFieldStart = false;
            }
        }
        if (quoted) throw new FormatException("Unclosed quoted field");
        if (field.Length > 0 || fields.Count > 0 || !atFieldStart)
        { fields.Add(field.ToString()); records.Add(fields); }
        if (records.Count == 0 || records[0].Count != 4 ||
            !records[0].SequenceEqual(new[] { "BatchId", "Species", "Count", "Note" }))
            throw new FormatException("Invalid header");
        var result = new List<NurseryBatch>();
        foreach (var r in records.Skip(1))
        {
            if (r.Count != 4 || !int.TryParse(r[2], out int count) || count < 0)
                throw new FormatException("Invalid nursery CSV record");
            result.Add(new NurseryBatch(r[0], r[1], count, r[3]));
        }
        return result;
    }
}
