var recipients = new HashSet<Recipient>();

var first = new Recipient
{
    TenantId = "acme",
    Email = " User@Example.com "
};

recipients.Add(first);

Normalize(first);

var sameLogicalRecipient = new Recipient
{
    TenantId = "acme",
    Email = "user@example.com"
};

var containsBeforeSecondAdd = recipients.Contains(sameLogicalRecipient);
var secondAddReturned = recipients.Add(sameLogicalRecipient);

Console.WriteLine($"ContainsBeforeSecondAdd={containsBeforeSecondAdd}");
Console.WriteLine($"SecondAddReturned={secondAddReturned}");
Console.WriteLine($"FinalCount={recipients.Count}");

foreach (var recipient in recipients)
{
    Console.WriteLine($"Recipient={recipient.TenantId}|{recipient.Email}");
}

static void Normalize(Recipient recipient)
{
    recipient.Email = recipient.Email.Trim().ToLowerInvariant();
}

public sealed class Recipient : IEquatable<Recipient>
{
    public required string TenantId { get; init; }
    public required string Email { get; set; }

    public bool Equals(Recipient? other)
        => other is not null
           && StringComparer.Ordinal.Equals(TenantId, other.TenantId)
           && StringComparer.OrdinalIgnoreCase.Equals(Email, other.Email);

    public override bool Equals(object? obj) => Equals(obj as Recipient);

    public override int GetHashCode()
        => HashCode.Combine(
            StringComparer.Ordinal.GetHashCode(TenantId),
            StringComparer.OrdinalIgnoreCase.GetHashCode(Email));
}
