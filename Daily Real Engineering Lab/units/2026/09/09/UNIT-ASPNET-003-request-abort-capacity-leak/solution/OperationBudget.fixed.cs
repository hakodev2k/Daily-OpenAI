sealed class OperationBudget : IDisposable
{
    private readonly CancellationTokenSource _timeout;
    private readonly CancellationTokenSource _linked;

    public OperationBudget(TimeSpan timeout, CancellationToken requestAborted)
    {
        _timeout = new CancellationTokenSource(timeout);
        _linked = CancellationTokenSource.CreateLinkedTokenSource(requestAborted, _timeout.Token);
        Token = _linked.Token;
    }

    public CancellationToken Token { get; }

    public void Dispose()
    {
        _linked.Dispose();
        _timeout.Dispose();
    }
}
