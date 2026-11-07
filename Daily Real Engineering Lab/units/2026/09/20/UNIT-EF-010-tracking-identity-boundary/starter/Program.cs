record Account(int Id, string CustomerId) { public bool CreditHold { get; set; } }

sealed class Store
{
    public Account Persisted { get; private set; } = new(10, "C-42");
    public Account LoadCopy() => new(Persisted.Id, Persisted.CustomerId) { CreditHold = Persisted.CreditHold };
    public void Save(Account tracked) => Persisted = new(tracked.Id, tracked.CustomerId) { CreditHold = tracked.CreditHold };
}

sealed class PersistenceScope(Store store)
{
    private readonly Dictionary<int, Account> _tracked = new();
    public string ScopeId { get; } = Guid.NewGuid().ToString("N")[..6];
    public Account Load(int id)
    {
        if (!_tracked.TryGetValue(id, out var entity)) _tracked[id] = entity = store.LoadCopy();
        return entity;
    }
    public void SaveChanges() { foreach (var entity in _tracked.Values) store.Save(entity); }
}

static class Program
{
    public static int Main(string[] args)
    {
        var store = new Store();
        var customerRepositoryScope = new PersistenceScope(store);
        var accountRepositoryScope = new PersistenceScope(store);

        var accountSeenByCustomerFlow = customerRepositoryScope.Load(10);
        var accountSeenByAccountFlow = accountRepositoryScope.Load(10);

        Console.WriteLine($"customer-flow scope={customerRepositoryScope.ScopeId} instance={System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(accountSeenByCustomerFlow)} hold={accountSeenByCustomerFlow.CreditHold}");
        Console.WriteLine($"account-flow  scope={accountRepositoryScope.ScopeId} instance={System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(accountSeenByAccountFlow)} hold={accountSeenByAccountFlow.CreditHold}");

        accountSeenByCustomerFlow.CreditHold = true;
        Console.WriteLine("business-step: credit hold requested");
        accountRepositoryScope.SaveChanges();
        Console.WriteLine($"persisted hold={store.Persisted.CreditHold}");

        if (args.Contains("reproduce"))
        {
            if (!store.Persisted.CreditHold) { Console.WriteLine("REPRODUCED"); return 2; }
            Console.WriteLine("NOT_REPRODUCED"); return 1;
        }

        if (args.Contains("verify"))
        {
            if (store.Persisted.CreditHold && ReferenceEquals(accountSeenByCustomerFlow, accountSeenByAccountFlow)) { Console.WriteLine("VERIFY_PASS"); return 0; }
            Console.WriteLine("VERIFY_FAIL"); return 3;
        }

        return 0;
    }
}
