using System;
using System.Collections.Generic;

public static class MessageSplitter
{
    // Provider accepts at most maxBytes of UTF-8 per segment.
    public static IReadOnlyList<string> Split(string message, int maxBytes)
    {
        ArgumentNullException.ThrowIfNull(message);
        if (maxBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxBytes));

        var segments = new List<string>();
        for (var start = 0; start < message.Length; start += maxBytes)
        {
            var length = Math.Min(maxBytes, message.Length - start);
            segments.Add(message.Substring(start, length));
        }

        return segments;
    }
}
