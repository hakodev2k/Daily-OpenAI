using ReusableCrateRefunds;

string mode = args.Length == 0 ? "demo" : args[0].ToLowerInvariant();

try
{
    if (mode is "demo" or "reproduce")
    {
        decimal single = DepositRefund.Calculate(1, 1.005m);
        decimal batch = DepositRefund.Calculate(3, 0.335m);
        decimal control = DepositRefund.Calculate(1, 1.015m);
        Console.WriteLine($"Single: amount=1.005, actual={single:F2}, contract=1.01");
        Console.WriteLine($"Batch:  amount=3*0.335, actual={batch:F2}, contract=1.01");
        Console.WriteLine($"Control: amount=1.015, actual={control:F2}, contract=1.02");

        if (mode == "reproduce")
        {
            if (single != 1.00m || batch != 1.00m || control != 1.02m)
                throw new InvalidOperationException("Starter symptom not observed; implementation or fixture has changed.");
            Console.WriteLine("REPRODUCED: two partner settlement contract mismatches.");
        }
    }
    else if (mode == "verify")
    {
        Expect("single midpoint", 1, 1.005m, 1.01m);
        Expect("batch rounding once", 3, 0.335m, 1.01m);
        Expect("midpoint with odd prior cent", 1, 1.015m, 1.02m);
        Expect("another midpoint", 1, 1.025m, 1.03m);
        Expect("negative reversal", -1, 1.005m, -1.01m);
        Expect("negative batch reversal", -3, 0.335m, -1.01m);
        Expect("non-midpoint below", 1, 2.674m, 2.67m);
        Expect("non-midpoint above", 1, 2.676m, 2.68m);
        Expect("zero quantity", 0, 1.005m, 0.00m);
        Expect("exact cent", 2, 0.50m, 1.00m);
        Expect("zero unit deposit", 3, 0m, 0m);
        bool rejected = false;
        try { _ = DepositRefund.Calculate(1, -0.01m); }
        catch (ArgumentOutOfRangeException) { rejected = true; }
        if (!rejected) throw new InvalidOperationException("Negative unitDeposit must be rejected.");
        Console.WriteLine("PASS: negative unitDeposit rejected.");
        Console.WriteLine("VERIFIED: 12 learner-starter behavior contracts passed.");
    }
    else
    {
        throw new ArgumentException("Mode must be demo, reproduce, or verify.");
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine("FAILED: " + ex.Message);
    Environment.ExitCode = 1;
}

static void Expect(string name, int quantity, decimal deposit, decimal expected)
{
    decimal actual = DepositRefund.Calculate(quantity, deposit);
    if (actual != expected)
        throw new InvalidOperationException($"{name}: expected {expected:F2}, actual {actual:F2}.");
    Console.WriteLine("PASS: " + name);
}
