using System.Text.Json;

var customer = new Customer("Mai", "+84-912-345-678");
Console.WriteLine($"Before: {customer}");

var renameOnlyJson = """{"displayName":"Mai Nguyen"}""";
var rename = JsonSerializer.Deserialize<UpdateCustomerRequest>(renameOnlyJson, JsonOptions)!;
ApplyUpdate(customer, rename);
Console.WriteLine($"After rename-only request: {customer}");

if (customer.PhoneNumber is null)
{
    Console.Error.WriteLine("FAIL: phoneNumber changed although the payload did not contain phoneNumber.");
    Environment.ExitCode = 1;
    return;
}

Console.WriteLine("PASS");

static void ApplyUpdate(Customer customer, UpdateCustomerRequest request)
{
    if (request.DisplayName is not null)
        customer.DisplayName = request.DisplayName;

    // Investigation note: which business intent does this runtime value represent?
    customer.PhoneNumber = request.PhoneNumber;
}

sealed class Customer(string displayName, string? phoneNumber)
{
    public string DisplayName { get; set; } = displayName;
    public string? PhoneNumber { get; set; } = phoneNumber;
    public override string ToString() => $"DisplayName={DisplayName}, PhoneNumber={PhoneNumber ?? "<null>"}";
}

sealed record UpdateCustomerRequest(string? DisplayName, string? PhoneNumber);

static class JsonOptions
{
    public static readonly JsonSerializerOptions Value = new(JsonSerializerDefaults.Web);
    public static implicit operator JsonSerializerOptions(JsonOptions _) => Value;
}
