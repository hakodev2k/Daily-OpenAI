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

        Task<string> stdoutTask = child.StandardOutput.ReadToEndAsync();
        Task<string> stderrTask = child.StandardError.ReadToEndAsync();
        Task exitTask = child.WaitForExitAsync();
        Task completion = Task.WhenAll(stdoutTask, stderrTask, exitTask);

        if (await Task.WhenAny(completion, Task.Delay(TimeSpan.FromSeconds(4))) != completion)
        {
            try
            {
                if (!child.HasExited) child.Kill(entireProcessTree: true);
            }
            catch (InvalidOperationException) { /* Child already exited. */ }
            try { await completion; } catch { /* Cleanup after forced termination. */ }
            Console.WriteLine("RENDER_TIMEOUT: renderer exceeded 4s; child terminated.");
            return 23;
        }

        await completion;
        string stdout = await stdoutTask;
        string stderr = await stderrTask;
        if (child.ExitCode != 0)
        {
            Console.WriteLine("CHILD_FAILED: exit=" + child.ExitCode);
            return 31;
        }
        if (stdout.Trim() != "rendered=" + bytes || stderr.Length != bytes)
        {
            Console.WriteLine("OUTPUT_MISMATCH: stdout=" + stdout.Trim()
                + " stderrLength=" + stderr.Length);
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
