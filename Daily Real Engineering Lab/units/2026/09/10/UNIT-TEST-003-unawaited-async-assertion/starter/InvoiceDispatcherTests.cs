namespace Invoice.Tests;

public sealed class InvoiceDispatcher
{
    public async Task DispatchAsync(CancellationToken cancellationToken = default)
    {
        await Task.Delay(25, cancellationToken);
        throw new InvoiceDispatchException("Provider rejected the invoice.");
    }
}

public sealed class InvoiceDispatchException : Exception
{
    public InvoiceDispatchException(string message) : base(message) { }
}

public sealed class InvoiceDispatcherTests
{
    [Fact]
    public void DispatchAsync_WhenProviderRejects_ThrowsExpectedException()
    {
        var sut = new InvoiceDispatcher();

        // Investigation note:
        // What does this assertion return, and who observes its completion?
        Assert.ThrowsAsync<InvoiceDispatchException>(() => sut.DispatchAsync());
    }
}
