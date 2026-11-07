using System.Net;
using System.Net.Http.Json;

namespace TransferLab;

public sealed record TransferCommand(string TransferId, string Isbn, int Quantity);

public sealed class TransferDispatcher
{
    private readonly HttpClient _client;

    public TransferDispatcher(HttpClient client) => _client = client;

    public async Task<HttpStatusCode> DispatchAsync(
        TransferCommand command,
        CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; attempt <= 2; attempt++)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "transfers")
            {
                Content = JsonContent.Create(command)
            };
            request.Headers.TryAddWithoutValidation("Idempotency-Key", command.TransferId);

            using var response = await _client.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable && attempt < 2)
            {
                continue;
            }

            return response.StatusCode;
        }

        throw new InvalidOperationException("Unexpected retry-loop exit.");
    }
}
