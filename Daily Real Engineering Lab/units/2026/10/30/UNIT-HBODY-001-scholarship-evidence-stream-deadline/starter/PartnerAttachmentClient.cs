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

        using var request = new HttpRequestMessage(HttpMethod.Get, resource);
        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            callerCancellation);

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
