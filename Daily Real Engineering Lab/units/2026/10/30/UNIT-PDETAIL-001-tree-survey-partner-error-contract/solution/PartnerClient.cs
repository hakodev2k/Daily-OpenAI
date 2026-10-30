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
        if (response.IsSuccessStatusCode)
            return new SyncReceipt(batchId);

        var status = response.StatusCode;
        var code = "UPSTREAM_HTTP_ERROR";
        string? trace = response.Headers.TryGetValues("X-Request-ID", out var values)
            ? values.FirstOrDefault() : null;

        if (string.Equals(response.Content.Headers.ContentType?.MediaType,
            "application/problem+json", StringComparison.OrdinalIgnoreCase))
        {
            // Bounded diagnostic read: do not buffer arbitrary upstream payloads.
            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream, Encoding.UTF8,
                detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: false);
            var buffer = new char[4096];
            var length = await reader.ReadBlockAsync(buffer.AsMemory(), cancellationToken);
            try
            {
                using var problem = JsonDocument.Parse(new string(buffer, 0, length));
                var root = problem.RootElement;
                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("code", out var codeValue)
                        && codeValue.ValueKind == JsonValueKind.String)
                        code = SafeToken(codeValue.GetString(), code);

                    if (root.TryGetProperty("traceId", out var traceValue)
                        && traceValue.ValueKind == JsonValueKind.String)
                        trace = SafeToken(traceValue.GetString(), trace ?? "none");
                }
            }
            catch (JsonException)
            {
                // Partner sent malformed/truncated problem JSON; use safe fallback.
            }
        }

        trace = trace is null ? null : SafeToken(trace, "none");
        throw new PartnerSyncException(status, code, trace, (int)status >= 500);
    }

    private static string SafeToken(string? value, string fallback)
    {
        if (string.IsNullOrWhiteSpace(value)) return fallback;
        var safe = new string(value.Take(64)
            .Where(c => char.IsAsciiLetterOrDigit(c) || c == '_' || c == '-').ToArray());
        return safe.Length == 0 ? fallback : safe;
    }
}
