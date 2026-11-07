using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

internal static class Program
{
    private const string ExpectedBody = "SCHOLARSHIP-EVIDENCE-V1";

    private static async Task<int> Main(string[] args)
    {
        string mode = args.Length == 1 ? args[0].ToLowerInvariant() : "demo";
        if (mode is not ("demo" or "reproduce" or "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run --project harness -- [demo|reproduce|verify]");
            return 2;
        }

        Probe fast = await RunProbeAsync("/fast", TimeSpan.FromMilliseconds(250), null);
        Probe slow = await RunProbeAsync("/slow", TimeSpan.FromMilliseconds(250), null);
        Probe callerCancelled = await RunProbeAsync("/slow", TimeSpan.FromSeconds(3), 90);

        Print("FAST", fast);
        Print("SLOW_BODY", slow);
        Print("CALLER_CANCEL", callerCancelled);

        bool fastOk = fast.Completed && fast.Body == ExpectedBody && fast.ElapsedMs < 1000;
        bool originalSymptom = slow.Completed && slow.Body == ExpectedBody && slow.ElapsedMs >= 1000
            && callerCancelled.Completed && callerCancelled.Body == ExpectedBody
            && callerCancelled.ElapsedMs >= 1000;
        bool fixedBehavior = slow.Cancelled && slow.ElapsedMs < 1000
            && callerCancelled.Cancelled && callerCancelled.ElapsedMs < 650;

        if (mode == "demo")
        {
            Console.WriteLine("Demo only. Run reproduce.ps1 before editing; verify.ps1 after editing.");
            return fastOk ? 0 : 1;
        }

        bool pass = fastOk && (mode == "reproduce" ? originalSymptom : fixedBehavior);
        Console.WriteLine(pass
            ? (mode == "reproduce" ? "REPRODUCED: slow body exceeded deadline" : "VERIFIED: deadline and caller cancellation honored")
            : (mode == "reproduce" ? "NOT REPRODUCED: inspect original starter and environment" : "NOT VERIFIED: deadline/cancellation or fast-body behavior failed"));
        return pass ? 0 : 1;
    }

    private static async Task<Probe> RunProbeAsync(string route, TimeSpan budget, int? cancelAfterMs)
    {
        using var client = new HttpClient(new SimulatedPartnerHandler())
        {
            Timeout = TimeSpan.FromMilliseconds(250)
        };
        using var caller = new CancellationTokenSource();
        if (cancelAfterMs is int ms)
        {
            caller.CancelAfter(ms);
        }

        var timer = Stopwatch.StartNew();
        try
        {
            string body = await PartnerAttachmentClient.FetchAsync(
                client, new Uri("https://partner.invalid" + route), budget, caller.Token);
            return new Probe(true, false, timer.ElapsedMilliseconds, body);
        }
        catch (OperationCanceledException)
        {
            return new Probe(false, true, timer.ElapsedMilliseconds, "");
        }
    }

    private static void Print(string name, Probe p)
    {
        Console.WriteLine($"{name}: completed={p.Completed}, cancelled={p.Cancelled}, elapsedMs={p.ElapsedMs}, body={p.Body}");
    }

    private sealed record Probe(bool Completed, bool Cancelled, long ElapsedMs, string Body);

    private sealed class SimulatedPartnerHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.RequestUri?.AbsolutePath is not ("/fast" or "/slow"))
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            TimeSpan bodyDelay = request.RequestUri.AbsolutePath == "/fast"
                ? TimeSpan.FromMilliseconds(15)
                : TimeSpan.FromMilliseconds(1200);

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                RequestMessage = request,
                Content = new StreamContent(new DeferredBodyStream(ExpectedBody, bodyDelay))
            };
            response.Content.Headers.ContentType =
                new MediaTypeHeaderValue("text/plain") { CharSet = "utf-8" };
            return Task.FromResult(response);
        }
    }

    private sealed class DeferredBodyStream : Stream
    {
        private readonly byte[] _bytes;
        private readonly TimeSpan _delay;
        private bool _firstRead = true;
        private int _position;

        public DeferredBodyStream(string content, TimeSpan delay)
        {
            _bytes = Encoding.UTF8.GetBytes(content);
            _delay = delay;
        }

        public override async ValueTask<int> ReadAsync(
            Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (_firstRead)
            {
                _firstRead = false;
                await Task.Delay(_delay, cancellationToken).ConfigureAwait(false);
            }

            int count = Math.Min(buffer.Length, _bytes.Length - _position);
            if (count == 0) return 0;
            _bytes.AsSpan(_position, count).CopyTo(buffer.Span);
            _position += count;
            return count;
        }

        public override Task<int> ReadAsync(
            byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException();
        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) =>
            throw new NotSupportedException();
        public override void SetLength(long value) =>
            throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException();
    }
}
