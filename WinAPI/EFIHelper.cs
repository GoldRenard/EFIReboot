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
using System.Collections.Generic;
using System.Text;
using EFIReboot.Model;

namespace EFIReboot.WinAPI {

    internal static class EFIHelper {
        private const string EFI_GLOBAL_VARIABLE = "{8BE4DF61-93CA-11D2-AA0D-00E098032B8C}";

        private const string EFI_TEST_VARIABLE = "{00000000-0000-0000-0000-000000000000}";

        private const string EFI_BOOT_ORDER = "BootOrder";

        private const string EFI_BOOT_CURRENT = "BootCurrent";

        private const string EFI_BOOT_NEXT = "BootNext";

        public static bool IsSupported() {
            if (NativeMethods.GetFirmwareType() != FirmwareType.FirmwareTypeUefi) {
                return false;
            }

            if (NativeMethods.GetFirmwareEnvironmentVariable(string.Empty, EFI_TEST_VARIABLE, null, 0) == 0) {
                uint lastError = NativeMethods.GetLastError();
                return NativeMethods.GetLastError() == 998;
            }
            return true;
        }

        public static uint ReadVariable(string name, ref ushort[] buffer) {
            buffer = new ushort[2048];
            uint size = (uint)(sizeof(ushort) * buffer.Length);
            uint dataSize = NativeMethods.GetFirmwareEnvironmentVariable(name, EFI_GLOBAL_VARIABLE, buffer, size);
            return dataSize / 2;
        }

        public static List<BootEntry> GetEntries() {
            ushort[] buffer = null;
            uint length = ReadVariable(EFI_BOOT_ORDER, ref buffer);
            List<BootEntry> entries = new List<BootEntry>();
            ushort currentEntryId = GetBootCurrent();
            ushort nextEntryId = GetBootNext();
            for (int i = 0; i < length; i++) {
                ushort entryId = buffer[i];
                entries.Add(new BootEntry() {
                    Id = entryId,
                    Name = GetBootEntryName(entryId),
                    IsCurrent = currentEntryId == entryId,
                    IsBootNext = nextEntryId == entryId
                });
            }
            return entries;
        }

        public static string GetBootEntryName(ushort entryId) {
            ushort[] buffer = null;
            uint length = ReadVariable(string.Format(BootEntry.INTERNAL_FORMAT, entryId), ref buffer);
            StringBuilder builder = new StringBuilder((int)length);
            for (int i = 3; i < length; i++) {
                if (buffer[i] == 0) break;
                builder.Append(Convert.ToChar(buffer[i]));
            }
            return builder.ToString();
        }

        public static ushort GetBootCurrent() {
            ushort[] buffer = null;
            uint length = ReadVariable(EFI_BOOT_CURRENT, ref buffer);
            return buffer[0];
        }

        public static ushort GetBootNext() {
            ushort[] buffer = null;
            uint length = ReadVariable(EFI_BOOT_NEXT, ref buffer);
            return buffer[0];
        }

        public static bool SetVariable(string name, ushort[] buffer) {
            uint size = (uint)(sizeof(ushort) * buffer.Length);
            bool result = NativeMethods.SetFirmwareEnvironmentVariable(name, EFI_GLOBAL_VARIABLE, buffer, size);
            if (result) {
                return result;
            } else {
                throw new InvalidOperationException("Unable to set variable");
            }
        }

        public static bool SetBootNext(ushort entryId) {
            return SetVariable(EFI_BOOT_NEXT, new ushort[] { entryId });
        }

        public static bool SetBootNext(BootEntry entry) {
            return SetBootNext(entry.Id);
        }
    }
}