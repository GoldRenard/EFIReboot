using System;
using System.Runtime.InteropServices;
using EFIReboot.WinAPI;

namespace EFIReboot {

    internal class NativeMethods {

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32.dll")]
        private static extern bool GetFirmwareType(ref FirmwareType FirmwareType);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern uint GetFirmwareEnvironmentVariable([MarshalAs(UnmanagedType.LPWStr)] string lpName, [MarshalAs(UnmanagedType.LPWStr)] string lpGuid, ushort[] pBuffer, uint nSize);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetFirmwareEnvironmentVariable([MarshalAs(UnmanagedType.LPWStr)] string lpName, [MarshalAs(UnmanagedType.LPWStr)] string lpGuid, ushort[] pValue, uint nSize);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref PrivelegeHelper.TokenPrivelege newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ExitWindowsEx(ExitWindows uFlags, ShutdownReason dwReason);

        public static FirmwareType GetFirmwareType() {
            FirmwareType type = FirmwareType.FirmwareTypeUnknown;
            if (GetFirmwareType(ref type)) {
                return type;
            }
            return FirmwareType.FirmwareTypeUnknown;
        }
    }
}