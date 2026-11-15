using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace AotForms
{
    public static class DllInjector
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

        private const uint PROCESS_ALL_ACCESS = 0x001F0FFF;
        private const uint MEM_COMMIT = 0x1000;
        private const uint MEM_RESERVE = 0x2000;
        private const uint PAGE_READWRITE = 0x04;
        private const uint INFINITE = 0xFFFFFFFF;

        /// <summary>
        /// Injects a DLL into a target process by process ID.
        /// </summary>
        public static bool InjectDll(int processId, string dllPath)
        {
            IntPtr hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, processId);
            if (hProcess == IntPtr.Zero)
                return false;

            try
            {
                // Allocate memory for DLL path
                IntPtr allocMem = VirtualAllocEx(hProcess, IntPtr.Zero, (uint)(dllPath.Length + 1), MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
                if (allocMem == IntPtr.Zero)
                    return false;

                // Write DLL path
                byte[] bytes = Encoding.ASCII.GetBytes(dllPath);
                if (!WriteProcessMemory(hProcess, allocMem, bytes, (uint)bytes.Length, out _))
                    return false;

                // Get LoadLibraryA address
                IntPtr loadLibraryAddr = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
                if (loadLibraryAddr == IntPtr.Zero)
                    return false;

                // Create remote thread to load DLL
                IntPtr hThread = CreateRemoteThread(hProcess, IntPtr.Zero, 0, loadLibraryAddr, allocMem, 0, IntPtr.Zero);
                if (hThread == IntPtr.Zero)
                    return false;

                // Wait for DLL to load
                WaitForSingleObject(hThread, 5000);
                CloseHandle(hThread);
                return true;
            }
            finally
            {
                CloseHandle(hProcess);
            }
        }

        /// <summary>
        /// Injects a DLL into the first process with the given name.
        /// </summary>
        public static bool InjectIntoGame(string processName, string dllPath)
        {
            var processes = Process.GetProcessesByName(processName);
            if (processes.Length == 0)
                return false;

            return InjectDll(processes[0].Id, dllPath);
        }
    }
}