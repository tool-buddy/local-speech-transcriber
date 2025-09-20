using System.Diagnostics;
using System.Runtime.InteropServices;
using ToolBuddy.LocalSpeechTranscriber.Application.Contracts;

namespace ToolBuddy.LocalSpeechTranscriber.Infrastructure.Stt.Whisper.Python
{
    public sealed class CrossPlatformPythonLocator : IPythonLocator
    {
        public bool TryGetPythonPath(
            out string? path)
        {
            if (TryProbe(
                    ["python"],
                    out path
                ))
                return true;

            if (TryProbe(
                    ["python3"],
                    out path
                ))
                return true;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                && TryProbe(
                    ["py", "-3"],
                    out path
                ))
                return true;

            path = null;
            return false;
        }

        private static bool TryProbe(
            string[] commandAndArgs,
            out string? path)
        {
            try
            {
                using Process process = new();
                process.StartInfo.FileName = commandAndArgs[0];
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                for (int i = 1; i < commandAndArgs.Length; i++)
                    process.StartInfo.ArgumentList.Add(commandAndArgs[i]);
                process.StartInfo.ArgumentList.Add("-c");
                process.StartInfo.ArgumentList.Add("import sys; print(sys.executable)");

                if (!process.Start())
                    path = null;
                else
                {
                    string? stdout = process.StandardOutput.ReadLine();

                    if (!process.WaitForExit((int)TimeSpan.FromSeconds(2).TotalMilliseconds))
                    {
                        try
                        {
                            process.Kill(true);
                        }
                        catch
                        {
                            // ignored
                        }

                        path = null;
                    }
                    else
                    {
                        if (process.ExitCode != 0
                            || String.IsNullOrWhiteSpace(stdout))
                            path = null;
                        else
                            path = stdout.Trim();
                    }
                }
            }
            catch
            {
                path = null;
            }

            return path != null;
        }
    }
}