using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

public static class MessageSplitter
{
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    public static IReadOnlyList<string> Split(string message, int maxBytes)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (maxBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxBytes));

        var segments = new List<string>();
        var current = new StringBuilder();
        var currentBytes = 0;
        var elements = StringInfo.GetTextElementEnumerator(message);

        while (elements.MoveNext())
        {
            var element = elements.GetTextElement();
            var bytes = StrictUtf8.GetByteCount(element);

            if (bytes > maxBytes)
                throw new ArgumentException(
                    "One text element exceeds the provider byte budget.", nameof(maxBytes));

            if (currentBytes > 0 && currentBytes + bytes > maxBytes)
            {
                segments.Add(current.ToString());
                current.Clear();
                currentBytes = 0;
            }

            current.Append(element);
            currentBytes += bytes;
        }

        if (currentBytes > 0)
            segments.Add(current.ToString());

        return segments;
    }
}
