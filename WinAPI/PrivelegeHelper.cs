using System;
using System.Runtime.InteropServices;

namespace EFIReboot.WinAPI {

    internal class PrivelegeHelper {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct TokenPrivelege {
            public int Count;
            public long Luid;
            public int Attr;
        }

        private const string SE_SYSTEM_ENVIRONMENT_NAME = "SeSystemEnvironmentPrivilege";

        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        internal const int SE_PRIVILEGE_ENABLED = 0x00000002;

        internal const int TOKEN_QUERY = 0x00000008;

        internal const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        public static void ObtainPrivileges(string privilege) {
            IntPtr hToken = IntPtr.Zero;

            if (!NativeMethods.OpenProcessToken(NativeMethods.GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref hToken)) {
                throw new InvalidOperationException("OpenProcessToken failed!");
            }

            TokenPrivelege tp;
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;

            if (!NativeMethods.LookupPrivilegeValue(null, privilege, ref tp.Luid)) {
                throw new InvalidOperationException("LookupPrivilegeValue failed!");
            }
            if (!NativeMethods.AdjustTokenPrivileges(hToken, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero)) {
                throw new InvalidOperationException("AdjustTokenPrivileges failed!");
            }
        }

        public static void ObtainSystemPrivileges() {
            ObtainPrivileges(SE_SYSTEM_ENVIRONMENT_NAME);
            ObtainPrivileges(SE_SHUTDOWN_NAME);
        }
    }
}