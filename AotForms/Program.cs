using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AotForms
{
    internal static class Program
    {
        private static int _loadState = 0;
        private static bool _comRegistered = false;

        private const int MaxRetries = 50;
        private const int RetryDelayMs = 100;

        [UnmanagedCallersOnly(EntryPoint = "Load")]
        public static void Load(nint pVM)
        {
            if (Interlocked.CompareExchange(ref _loadState, 1, 0) != 0)
                return;

            try
            {
                /*
                 * IMPORTANT:
                 * Load() receives pVM from the native caller.
                 *
                 * If pVM == 0, we cannot magically obtain a new
                 * VM pointer here because this method has no VM
                 * lookup function.
                 *
                 * The 50 retries below therefore retry the
                 * initialization attempt only if the caller
                 * supplies another valid pVM through another
                 * Load() invocation.
                 */

                if (pVM == nint.Zero)
                {
                    Interlocked.Exchange(ref _loadState, 0);

                    //ShowMessage(
                    //    "The emulator VM is not ready yet.\n\n" +
                    //    "The loader received a NULL VM pointer.\n\n" +
                    //    "Please wait until the game has completely " +
                    //    "started and try again.",
                    //    "VM Not Ready",
                    //    MessageBoxIcon.Warning);

                    return;
                }

                // VM is valid.
                InternalMemory.Initialize(pVM);

                Process process = Process.GetCurrentProcess();

                if (!_comRegistered)
                {
                    ComWrappers.RegisterForMarshalling(
                        WinFormsComInterop.WinFormsComWrappers.Instance);

                    _comRegistered = true;
                }

                Application.SetHighDpiMode(
                    HighDpiMode.PerMonitorV2);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Application.Run(
                    new Form1(process.MainWindowHandle));
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref _loadState, 0);

                //ShowMessage(
                //    "Initialization failed.\n\n" +
                //    ex,
                //    "Initialization Error",
                //    MessageBoxIcon.Error);
            }
        }

        private static void ShowMessage(
            string message,
            string title,
            MessageBoxIcon icon)
        {
            try
            {
            //    MessageBox.Show(
            //        message,
            //        title,
            //        MessageBoxButtons.OK,
            //        icon);
            }
            catch
            {
                // Ignore secondary UI errors.
            }
        }
    }
}