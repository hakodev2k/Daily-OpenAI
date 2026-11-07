using System.Diagnostics;

namespace CaptionPipelineLab;

public sealed record ConversionResult(int ExitCode, string StandardOutput, string StandardError);

public static class ConversionRunner
{
    public static async Task<ConversionResult> RunAsync(CancellationToken cancellationToken = default)
    {
        using var process = new Process
        {
            StartInfo = new ProcessStartInfo("dotnet")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };
        process.StartInfo.ArgumentList.Add(typeof(ConversionRunner).Assembly.Location);
        process.StartInfo.ArgumentList.Add("--worker");
        if (!process.Start()) throw new InvalidOperationException("Converter did not start.");

        using var deadline = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        deadline.CancelAfter(TimeSpan.FromSeconds(10));
        Task<string> stdout = process.StandardOutput.ReadToEndAsync(deadline.Token);
        Task<string> stderr = process.StandardError.ReadToEndAsync(deadline.Token);

        try
        {
            await process.WaitForExitAsync(deadline.Token);
            var caption = await stdout;
            var diagnostic = await stderr;
            return new ConversionResult(process.ExitCode, caption, diagnostic);
        }
        catch (OperationCanceledException ex)
        {
            if (!process.HasExited)
            {
                try { process.Kill(entireProcessTree: true); }
                catch (InvalidOperationException) { /* Already exited. */ }
            }
            await process.WaitForExitAsync(CancellationToken.None);
            if (!cancellationToken.IsCancellationRequested)
                throw new TimeoutException("Converter exceeded the 10-second deadline.", ex);
            throw;
        }
    }
}
