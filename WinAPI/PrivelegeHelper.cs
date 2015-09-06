// ======================================================================
// EFI REBOOT
// Copyright (C) 2015 Ilya Egorov (goldrenard@gmail.com)

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// ======================================================================

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