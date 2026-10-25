using System.Net;

namespace TransferLab;

internal sealed record ObservedRequest(string? IdempotencyKey, string Body, string Path);

internal sealed class ScriptedPartnerHandler : HttpMessageHandler
{
    private readonly Queue<HttpStatusCode> _statuses;
    public List<ObservedRequest> Received { get; } = new();

    public ScriptedPartnerHandler(params HttpStatusCode[] statuses)
    {
        if (statuses.Length == 0) throw new ArgumentException("Provide at least one response.", nameof(statuses));
        _statuses = new Queue<HttpStatusCode>(statuses);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var body = request.Content is null
            ? string.Empty
            : await request.Content.ReadAsStringAsync(cancellationToken);

        var key = request.Headers.TryGetValues("Idempotency-Key", out var values)
            ? values.SingleOrDefault()
            : null;

        Received.Add(new ObservedRequest(key, body, request.RequestUri?.AbsolutePath ?? string.Empty));

        var status = _statuses.Count > 0
            ? _statuses.Dequeue()
            : HttpStatusCode.InternalServerError;

        return new HttpResponseMessage(status);
    }
}
