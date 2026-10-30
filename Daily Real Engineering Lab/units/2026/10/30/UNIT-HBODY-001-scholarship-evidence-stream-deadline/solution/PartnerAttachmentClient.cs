using System.Net.Http;

public static class PartnerAttachmentClient
{
    public static async Task<string> FetchAsync(
        HttpClient client,
        Uri resource,
        TimeSpan budget,
        CancellationToken callerCancellation)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(resource);
        if (budget <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(budget));

        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(callerCancellation);
        deadline.CancelAfter(budget);

        using var request = new HttpRequestMessage(HttpMethod.Get, resource);
        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            deadline.Token);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(deadline.Token);
    }
}
