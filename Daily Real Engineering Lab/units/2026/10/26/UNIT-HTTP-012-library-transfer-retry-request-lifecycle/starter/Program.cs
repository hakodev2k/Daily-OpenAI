using System.Net;

namespace TransferLab;

internal static class Program
{
    private static readonly TransferCommand Command = new("transfer-20261026-001", "9780132350884", 3);

    private static async Task<int> Main(string[] args)
    {
        var mode = args.Length == 2 && args[0] == "--mode" ? args[1] : "smoke";
        return mode switch
        {
            "smoke" => await SmokeAsync(),
            "reproduce" => await ReproduceAsync(),
            "verify" => await VerifyAsync(),
            _ => Fail("Unknown mode. Use --mode smoke|reproduce|verify.")
        };
    }

    private static HttpClient Client(ScriptedPartnerHandler handler) =>
        new(handler) { BaseAddress = new Uri("https://library-partner.invalid/api/") };

    private static async Task<int> SmokeAsync()
    {
        using var client = Client(new ScriptedPartnerHandler(HttpStatusCode.Accepted));
        var status = await new TransferDispatcher(client).DispatchAsync(Command);
        Console.WriteLine($"smoke_status={(int)status}");
        return status == HttpStatusCode.Accepted ? 0 : 1;
    }

    private static async Task<int> ReproduceAsync()
    {
        var handler = new ScriptedPartnerHandler(HttpStatusCode.ServiceUnavailable, HttpStatusCode.Accepted);
        using var client = Client(handler);
        try
        {
            var status = await new TransferDispatcher(client).DispatchAsync(Command);
            Console.WriteLine($"unexpected_status={(int)status}; partner_calls={handler.Received.Count}");
            return Fail("The original failure did not reproduce; starter may already be fixed.");
        }
        catch (InvalidOperationException ex) when (handler.Received.Count == 1)
        {
            Console.WriteLine($"REPRODUCED: exception={ex.GetType().Name}; partner_calls={handler.Received.Count}; first_status=503");
            Console.WriteLine($"first_request_path={handler.Received[0].Path}; body_bytes={System.Text.Encoding.UTF8.GetByteCount(handler.Received[0].Body)}");
            return 0;
        }
        catch (Exception ex)
        {
            return Fail($"Unexpected symptom: {ex.GetType().Name}; partner_calls={handler.Received.Count}");
        }
    }

    private static async Task<int> VerifyAsync()
    {
        try
        {
            var transient = new ScriptedPartnerHandler(HttpStatusCode.ServiceUnavailable, HttpStatusCode.Accepted);
            using (var client = Client(transient))
            {
                var status = await new TransferDispatcher(client).DispatchAsync(Command);
                if (status != HttpStatusCode.Accepted || transient.Received.Count != 2)
                    return Fail($"503->202: status={(int)status}; calls={transient.Received.Count}; expected 202 and 2.");

                var first = transient.Received[0];
                if (string.IsNullOrWhiteSpace(first.Body) ||
                    !first.Body.Contains(Command.Isbn, StringComparison.Ordinal) ||
                    transient.Received.Any(x => x.Body != first.Body ||
                        x.IdempotencyKey != Command.TransferId ||
                        x.Path != "/api/transfers"))
                    return Fail("Payload, path or stable idempotency contract changed across attempts.");
            }

            var badRequest = new ScriptedPartnerHandler(HttpStatusCode.BadRequest, HttpStatusCode.Accepted);
            using (var client = Client(badRequest))
            {
                var status = await new TransferDispatcher(client).DispatchAsync(Command);
                if (status != HttpStatusCode.BadRequest || badRequest.Received.Count != 1)
                    return Fail("400 must not be retried.");
            }

            var exhausted = new ScriptedPartnerHandler(HttpStatusCode.ServiceUnavailable, HttpStatusCode.ServiceUnavailable);
            using (var client = Client(exhausted))
            {
                var status = await new TransferDispatcher(client).DispatchAsync(Command);
                if (status != HttpStatusCode.ServiceUnavailable || exhausted.Received.Count != 2)
                    return Fail("Retry budget must be exactly two attempts.");
            }

            Console.WriteLine("PASS: 503->202 retried; 400 stopped; 503->503 bounded; body/path/key stable.");
            return 0;
        }
        catch (Exception ex)
        {
            return Fail($"Verification exception: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static int Fail(string message)
    {
        Console.Error.WriteLine($"FAIL: {message}");
        return 1;
    }
}
