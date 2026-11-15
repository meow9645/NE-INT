using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

namespace AotForms
{
    internal class Adb
    {
        string _path;
        ConcurrentQueue<string> _outputQueue = new ConcurrentQueue<string>();
        Process _process;

        internal Adb(string path)
        {
            _path = path;
        }

        internal async Task Kill()
        {
            var adbProcesses = Process.GetProcessesByName("adb")
                .Concat(Process.GetProcessesByName("HD-Adb"));

            foreach (var adbProcess in adbProcesses)
            {
                try
                {
                    adbProcess.Kill();
                    await adbProcess.WaitForExitAsync();
                }
                catch { /* Ignore */ }
            }
        }

        internal async Task<bool> Start()
        {
            return await Task.Run(() =>
            {
                // ---- DISKLESS ENVIRONMENT (NO FILES) ----
                // Set environment variables at the process level – inherited by all children.
                Environment.SetEnvironmentVariable("ADB_TRACE", "0", EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("ADB_LOG_DIR", "NUL", EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("TEMP", "NUL", EnvironmentVariableTarget.Process);
                Environment.SetEnvironmentVariable("TMP", "NUL", EnvironmentVariableTarget.Process);

                // ---- Kill old server and start fresh (without -L, to preserve reliability) ----
                ExecuteAdbCommand("kill-server");
                ExecuteAdbCommand("devices");

                // ---- Persistent shell process (YOUR ORIGINAL LOGIC) ----
                _process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = _path,
                        Arguments = "shell \"getprop ro.secure ; /boot/android/android/system/xbin/bstk/su\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                // Explicitly set environment for the persistent process too.
                _process.StartInfo.EnvironmentVariables["ADB_TRACE"] = "0";
                _process.StartInfo.EnvironmentVariables["ADB_LOG_DIR"] = "NUL";
                _process.StartInfo.EnvironmentVariables["TEMP"] = "NUL";
                _process.StartInfo.EnvironmentVariables["TMP"] = "NUL";

                _process.OutputDataReceived += Receiver;
                _process.ErrorDataReceived += Receiver;

                _process.Start();
                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();

                _process.StandardInput.AutoFlush = true;

                // Wait for output
                for (int i = 0; i < 5000; i++)
                {
                    if (!_outputQueue.IsEmpty) return true;
                    System.Threading.Thread.Sleep(1);
                }

                return false;
            });
        }

        void Receiver(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                _outputQueue.Enqueue(e.Data);
        }

        internal async Task<uint> FindModule(string process, string module)
        {
            return await Task.Run(async () =>
            {
                _outputQueue = new ConcurrentQueue<string>();

                _process.StandardInput.WriteLine("ps");

                string pid = await WaitForOutputAndExtractPID(process);
                if (string.IsNullOrEmpty(pid)) return 0U;

                _outputQueue = new ConcurrentQueue<string>();
                _process.StandardInput.WriteLine($"cat /proc/{pid}/maps | grep {module}");

                string address = await WaitForOutput();
                if (string.IsNullOrEmpty(address)) return 0U;

                _process.Kill();
                await _process.WaitForExitAsync();
                await Kill();

                return Convert.ToUInt32(address.Split('-')[0], 16);
            });
        }

        async Task<string> WaitForOutput()
        {
            for (int i = 0; i < 5000; i++)
            {
                if (_outputQueue.TryDequeue(out string result))
                    return result;
                await Task.Delay(1);
            }
            return null;
        }

        async Task<string> WaitForOutputAndExtractPID(string process)
        {
            for (int i = 0; i < 5000; i++)
            {
                if (_outputQueue.TryDequeue(out string result))
                {
                    var procLines = result.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var procLine in procLines)
                    {
                        if (procLine.Contains(process))
                        {
                            var parts = procLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length > 1) return parts[1];
                        }
                    }
                }
                await Task.Delay(1);
            }
            return null;
        }

        void ExecuteAdbCommand(string command)
        {
            using (var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = _path,
                    Arguments = command,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    CreateNoWindow = true
                }
            })
            {
                proc.StartInfo.EnvironmentVariables["ADB_TRACE"] = "0";
                proc.StartInfo.EnvironmentVariables["ADB_LOG_DIR"] = "NUL";
                proc.StartInfo.EnvironmentVariables["TEMP"] = "NUL";
                proc.StartInfo.EnvironmentVariables["TMP"] = "NUL";

                proc.Start();
                proc.WaitForExit();
            }
        }

        // ---- Empty method for compatibility – no folder was ever created ----
        public void CleanTempFolder()
        {
            // Nothing to clean.
        }
    }
}