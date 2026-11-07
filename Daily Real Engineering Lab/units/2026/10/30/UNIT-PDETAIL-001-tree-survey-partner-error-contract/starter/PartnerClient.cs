using System.Net;
using System.Text;
using System.Text.Json;

namespace TreeSurveyLab;

public sealed record SyncReceipt(string BatchId);

public sealed class PartnerSyncException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string ProblemCode { get; }
    public string? TraceId { get; }
    public bool IsRetryable { get; }

    public PartnerSyncException(HttpStatusCode statusCode, string problemCode, string? traceId, bool isRetryable)
        : base($"Partner sync rejected: {(int)statusCode}; code={problemCode}; trace={traceId ?? "none"}")
    {
        StatusCode = statusCode;
        ProblemCode = problemCode;
        TraceId = traceId;
        IsRetryable = isRetryable;
    }
}

public sealed class PartnerClient
{
    private readonly HttpClient _http;

    public PartnerClient(HttpClient http) => _http = http;

    public async Task<SyncReceipt> SubmitAsync(string batchId, CancellationToken cancellationToken = default)
    {
        using var content = new StringContent(
            JsonSerializer.Serialize(new { batchId }), Encoding.UTF8, "application/json");
        using var response = await _http.PostAsync("/api/batches", content, cancellationToken);
        response.EnsureSuccessStatusCode();
        return new SyncReceipt(batchId);
    }
}
