internal static class PartnerClient
{
    private static readonly Uri Endpoint = new("https://partner.example.test/v1/validate");

    public static Uri BuildValidationUri(string issuedPass)
    {
        ArgumentException.ThrowIfNullOrEmpty(issuedPass);
        return new Uri($"{Endpoint}?pass={issuedPass}", UriKind.Absolute);
    }
}
