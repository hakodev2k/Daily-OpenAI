using System.Text.Json;

var customer = new Customer("Mai", "+84-912-345-678");
Console.WriteLine($"Before: {customer}");
var json = """{"displayName":"Mai Nguyen"}""";
var request = JsonSerializer.Deserialize<UpdateCustomerRequest>(json, new JsonSerializerOptions(JsonSerializerDefaults.Web))!;
ApplyUpdate(customer, request);
Console.WriteLine($"After: {customer}");
if (customer.PhoneNumber is null) { Console.Error.WriteLine("FAIL: phoneNumber changed although payload omitted it."); Environment.ExitCode = 1; return; }
Console.WriteLine("PASS");

static void ApplyUpdate(Customer customer, UpdateCustomerRequest request)
{
    if (request.DisplayName is not null) customer.DisplayName = request.DisplayName;
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
