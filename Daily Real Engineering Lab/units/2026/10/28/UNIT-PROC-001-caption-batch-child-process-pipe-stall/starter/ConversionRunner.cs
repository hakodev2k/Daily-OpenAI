using System.Diagnostics;

namespace CaptionPipelineLab;

public sealed record ConversionResult(int ExitCode, string StandardOutput, string StandardError);

public static class ConversionRunner
{
    public static Task<ConversionResult> RunAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
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

        if (!process.WaitForExit(2_500))
        {
            try { process.Kill(entireProcessTree: true); }
            catch (InvalidOperationException) { /* Process already exited. */ }
            process.WaitForExit();
            throw new TimeoutException("Converter exceeded the 2.5-second exit deadline.");
        }

        var caption = process.StandardOutput.ReadToEnd();
        var diagnostic = process.StandardError.ReadToEnd();
        return Task.FromResult(new ConversionResult(process.ExitCode, caption, diagnostic));
    }
}
