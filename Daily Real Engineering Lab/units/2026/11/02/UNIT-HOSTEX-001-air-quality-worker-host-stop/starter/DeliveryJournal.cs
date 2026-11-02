using System.Collections.Concurrent;

public sealed class DeliveryJournal
{
    public ConcurrentQueue<string> Attempts { get; } = new();
    public ConcurrentQueue<string> Published { get; } = new();
    public ConcurrentQueue<string> Failed { get; } = new();
}
