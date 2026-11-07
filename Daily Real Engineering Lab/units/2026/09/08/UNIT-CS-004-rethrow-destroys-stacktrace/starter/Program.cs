class PaymentRepository
{
    public Task LoadAsync()
    {
        ThrowFromRepository();
        return Task.CompletedTask;
    }

    private static void ThrowFromRepository()
    {
        throw new InvalidOperationException("Payment record is corrupt");
    }
}

class PaymentService
{
    private readonly PaymentRepository _repository = new();

    public async Task ExecuteAsync()
    {
        try
        {
            await _repository.LoadAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine("SERVICE_LOG: adding payment context");
            throw ex;
        }
    }
}

try
{
    await new PaymentService().ExecuteAsync();
}
catch (Exception ex)
{
    Console.WriteLine("EXCEPTION_TYPE=" + ex.GetType().Name);
    Console.WriteLine("MESSAGE=" + ex.Message);
    Console.WriteLine("STACK_BEGIN");
    Console.WriteLine(ex.StackTrace);
    Console.WriteLine("STACK_END");
}
