# Reference Solution — inspect only after reproducing the issue and attempting your own fix

## 1. Symptoms

Repository method returns normally, but enumeration later throws `ObjectDisposedException`.

## 2. Evidence

The exception occurs at `ToListAsync()`, not while composing the LINQ expression. The query provider still depends on the `DbContext` that created it.

## 3. Root cause

`IQueryable` is deferred. `BuildLowStockQuery` returns an unevaluated EF Core query while its locally-owned `DbContext` is disposed when the method exits. The caller later tries to execute that query through a disposed context.

## 4. Why the fix works

Materialize the query while the repository-owned context is still alive and return materialized data across the lifetime boundary.

## 5. How to verify

Run `./verify.ps1`. It validates the learner-editable `starter/`, expects exit code 0, exactly two low-stock products, and no disposed-context marker.

## 6. Alternative fixes

- Let a higher-level request scope own the `DbContext` and ensure enumeration occurs inside that scope.
- Expose an application-level query method returning `Task<IReadOnlyList<T>>` instead of leaking provider-backed `IQueryable`.
- For genuinely composable queries, make lifetime ownership explicit and keep composition/execution inside a well-defined unit of work.

## 7. Wrong or misleading fixes

- Catching `ObjectDisposedException` and retrying with another context hides the lifetime design flaw.
- Removing `Dispose`/`using` without defining ownership can turn a deterministic bug into a resource-lifetime leak.
- Calling `AsEnumerable()` does not materialize the EF query; execution can still happen after the context is gone.

## 8. Production implications

Returning provider-backed `IQueryable` across architectural boundaries couples callers to EF Core execution semantics, lifetime, translation capability, and database behavior. This can make failures appear far from the data-access code that caused them.

## 9. Trade-offs

Materializing in the repository gives a clear lifetime boundary but reduces downstream query composition. Passing a scoped `DbContext` upward preserves composition but increases coupling and requires disciplined scope ownership.

## 10. What a Senior engineer should notice

The important issue is not merely “call `ToListAsync` earlier”. It is deciding **who owns query execution and resource lifetime**. The code should make that ownership obvious enough that a future refactor cannot accidentally move enumeration outside the valid scope.
