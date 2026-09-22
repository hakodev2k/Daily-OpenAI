record PaymentResult(string TenantId, string PaymentId, decimal Amount);

sealed class PaymentService
{
    private readonly Dictionary<string, PaymentResult> _idempotency = new();
    private int _sequence;

    public PaymentResult Create(string tenantId, string operation, string idempotencyKey, decimal amount)
    {
        // BUG nằm ở identity boundary của idempotency record.
        // Đừng sửa bằng cách xóa cache hoặc generate key ngẫu nhiên.
        var storageKey = idempotencyKey;

        if (_idempotency.TryGetValue(storageKey, out var previous))
            return previous;

        var result = new PaymentResult(tenantId, $"PAY-{++_sequence:000}", amount);
        _idempotency[storageKey] = result;
        return result;
    }

    public int CreatedPayments => _sequence;
}

var service = new PaymentService();
var a1 = service.Create("tenant-a", "create-payment", "retry-42", 100m);
var b1 = service.Create("tenant-b", "create-payment", "retry-42", 250m);
var aRetry = service.Create("tenant-a", "create-payment", "retry-42", 100m);

Console.WriteLine($"A first : {a1}");
Console.WriteLine($"B first : {b1}");
Console.WriteLine($"A retry : {aRetry}");
Console.WriteLine($"Created payments: {service.CreatedPayments}");

var isolated = a1.TenantId == "tenant-a" && b1.TenantId == "tenant-b" && a1.PaymentId != b1.PaymentId;
var retryStable = aRetry.PaymentId == a1.PaymentId;
Console.WriteLine(isolated && retryStable && service.CreatedPayments == 2 ? "CONTRACT_OK" : "CROSS_TENANT_COLLISION");
