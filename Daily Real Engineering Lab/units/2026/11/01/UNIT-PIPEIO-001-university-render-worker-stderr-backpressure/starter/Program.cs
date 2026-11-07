using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace ProcessPipeLab;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        int bytes = ReadInt(args, "--bytes=", 524288);
        int childExit = ReadInt(args, "--child-exit=", 0);
        if (bytes < 1 || bytes > 1048576 || childExit < 0 || childExit > 20)
        {
            Console.Error.WriteLine("Invalid arguments.");
            return 64;
        }

        if (args.Contains("--child", StringComparer.Ordinal))
        {
            Console.Error.Write(new string('E', bytes));
            Console.Error.Flush();
            Console.Out.WriteLine("rendered=" + bytes);
            return childExit;
        }

        using var child = new Process();
        child.StartInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        child.StartInfo.ArgumentList.Add(Assembly.GetExecutingAssembly().Location);
        child.StartInfo.ArgumentList.Add("--child");
        child.StartInfo.ArgumentList.Add("--bytes=" + bytes.ToString(CultureInfo.InvariantCulture));
        child.StartInfo.ArgumentList.Add("--child-exit=" + childExit.ToString(CultureInfo.InvariantCulture));
        child.Start();

        // Existing worker adapter: capture renderer output for the audit record.
        var captureTask = Task.Run(() =>
        {
            string stdout = child.StandardOutput.ReadToEnd();
            string stderr = child.StandardError.ReadToEnd();
            child.WaitForExit();
            return (stdout, stderr);
        });

        if (await Task.WhenAny(captureTask, Task.Delay(TimeSpan.FromSeconds(4))) != captureTask)
        {
            try
            {
                if (!child.HasExited) child.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException) { /* Child already exited. */ }
            try { await captureTask; } catch { /* Cleanup after forced termination. */ }
            Console.WriteLine("DEADLOCK_REPRODUCED: renderer exceeded 4s; child terminated.");
            return 23;
        }

        var captured = await captureTask;
        if (child.ExitCode != 0)
        {
            Console.WriteLine("CHILD_FAILED: exit=" + child.ExitCode);
            return 31;
        }
        if (captured.stdout.Trim() != "rendered=" + bytes || captured.stderr.Length != bytes)
        {
            Console.WriteLine("OUTPUT_MISMATCH: stdout=" + captured.stdout.Trim()
                + " stderrLength=" + captured.stderr.Length);
            return 24;
        }

        Console.WriteLine("STREAMS_DRAINED: bytes=" + bytes + " exit=0");
        return 0;
    }

    private static int ReadInt(string[] args, string prefix, int fallback)
    {
        string? raw = args.FirstOrDefault(arg => arg.StartsWith(prefix, StringComparison.Ordinal));
        if (raw is null) return fallback;
        return int.TryParse(raw[prefix.Length..], NumberStyles.None, CultureInfo.InvariantCulture,
            out int value) ? value : -1;
    }
}
