using System.Runtime.InteropServices;
using JetBrains.Annotations;

namespace Gravy.Ansi;

[PublicAPI]
public static class WindowsConsole
{
    public static bool TryEnableVirtualTerminalProcessing()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return false;
        try
        {
            var stdHandle = NativeMethods.GetStdHandle(-11);
            NativeMethods.GetConsoleMode(stdHandle, out var mode);
            NativeMethods.SetConsoleMode(stdHandle, mode | 4);
            NativeMethods.GetConsoleMode(stdHandle, out mode);
            return (mode & 4) == 4;
        }
        catch (DllNotFoundException)
        {
            return false;
        }
        catch (EntryPointNotFoundException)
        {
            return false;
        }
    }

    private static class NativeMethods
    {
        private const string Kernel32 = "kernel32.dll";

        [DllImport(Kernel32, SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

        [DllImport(Kernel32, SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr handle, out int mode);

        [DllImport(Kernel32, SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int handle);
    }
}