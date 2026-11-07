using System.Text;

namespace TrialImport;

public static class CsvParser
{
    public static IReadOnlyList<string[]> ParseRecords(string csv)
    {
        ArgumentNullException.ThrowIfNull(csv);

        var records = new List<string[]>();
        var fields = new List<string>();
        var field = new StringBuilder();
        var insideQuotes = false;
        var closedQuote = false;

        for (var i = 0; i < csv.Length; i++)
        {
            var c = csv[i];

            if (insideQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < csv.Length && csv[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        insideQuotes = false;
                        closedQuote = true;
                    }
                }
                else
                {
                    field.Append(c);
                }
                continue;
            }

            if (c == ',' || c == '\r' || c == '\n')
            {
                fields.Add(field.ToString());
                field.Clear();
                closedQuote = false;

                if (c == '\r' || c == '\n')
                {
                    if (c == '\r' && i + 1 < csv.Length && csv[i + 1] == '\n')
                        i++;
                    records.Add(fields.ToArray());
                    fields.Clear();
                }
                continue;
            }

            if (closedQuote)
                throw new FormatException("Unexpected character after closing CSV quote.");

            if (c == '"')
            {
                if (field.Length != 0)
                    throw new FormatException("Unexpected quote in unquoted CSV field.");
                insideQuotes = true;
                continue;
            }

            field.Append(c);
        }

        if (insideQuotes)
            throw new FormatException("Unterminated quoted CSV field.");

        if (field.Length > 0 || fields.Count > 0 || closedQuote)
        {
            fields.Add(field.ToString());
            records.Add(fields.ToArray());
        }

        return records;
    }
}
