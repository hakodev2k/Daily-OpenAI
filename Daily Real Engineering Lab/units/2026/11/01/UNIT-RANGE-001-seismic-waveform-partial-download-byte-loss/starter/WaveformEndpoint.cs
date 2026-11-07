using System.Globalization;
using System.Text;

internal static class WaveformEndpoint
{
    private static readonly byte[] Waveform = Encoding.ASCII.GetBytes("ABCDEFGHIJ");

    public static async Task HandleAsync(HttpContext context)
    {
        if (context.Request.RouteValues["id"]?.ToString() != "SEIS-42")
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            return;
        }

        context.Response.ContentType = "application/octet-stream";
        context.Response.Headers["Accept-Ranges"] = "bytes";
        var header = context.Request.Headers["Range"].ToString();

        // This endpoint intentionally supports one byte range per request.
        // For unsupported range units or multipart ranges, send the full representation.
        if (!header.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase)
            || header.Contains(','))
        {
            await WriteFullAsync(context);
            return;
        }

        var parts = header[6..].Split('-', 2);
        if (parts.Length != 2)
        {
            await WriteFullAsync(context);
            return;
        }

        int start;
        int end;
        if (parts[0].Length == 0)
        {
            if (!TryUnsigned(parts[1], out int suffixLength) || suffixLength == 0)
            {
                RejectRange(context);
                return;
            }
            start = Math.Max(0, Waveform.Length - suffixLength);
            end = Waveform.Length - 1;
        }
        else
        {
            if (!TryUnsigned(parts[0], out start))
            {
                await WriteFullAsync(context);
                return;
            }

            if (parts[1].Length == 0)
            {
                end = Waveform.Length - 1;
            }
            else if (!TryUnsigned(parts[1], out end))
            {
                await WriteFullAsync(context);
                return;
            }

            if (start >= Waveform.Length || end < start)
            {
                RejectRange(context);
                return;
            }
            end = Math.Min(end, Waveform.Length - 1);
        }

        context.Response.StatusCode = StatusCodes.Status206PartialContent;
        context.Response.Headers["Content-Range"] =
            "bytes " + start + "-" + end + "/" + Waveform.Length;

        // Segment data is assembled separately from the advertised metadata.
        await context.Response.Body.WriteAsync(Waveform.AsMemory(start, end - start));
    }

    private static bool TryUnsigned(string value, out int parsed) =>
        int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out parsed);

    private static async Task WriteFullAsync(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.Body.WriteAsync(Waveform.AsMemory());
    }

    private static void RejectRange(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status416RangeNotSatisfiable;
        context.Response.Headers["Content-Range"] = "bytes */" + Waveform.Length;
    }
}
