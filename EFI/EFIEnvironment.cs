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
using System.IO;
using EFIReboot.Model;
using EFIReboot.WinAPI;

namespace EFIReboot.EFI {

    internal static class EFIEnvironment {
        private const string EFI_GLOBAL_VARIABLE = "{8BE4DF61-93CA-11D2-AA0D-00E098032B8C}";

        private const string EFI_TEST_VARIABLE = "{00000000-0000-0000-0000-000000000000}";

        private const string EFI_BOOT_ORDER = "BootOrder";

        private const string EFI_BOOT_CURRENT = "BootCurrent";

        private const string EFI_BOOT_NEXT = "BootNext";

        private const string LOAD_OPTION_FORMAT = "Boot{0:X4}";

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

        public static uint ReadVariable(string name, ref byte[] buffer) {
            buffer = new byte[2048];
            return NativeMethods.GetFirmwareEnvironmentVariable(name, EFI_GLOBAL_VARIABLE, buffer, (uint)buffer.Length);
        }

        public static List<BootEntry> GetEntries() {
            byte[] buffer = null;
            uint length = ReadVariable(EFI_BOOT_ORDER, ref buffer);
            List<BootEntry> entries = new List<BootEntry>();
            ushort currentEntryId = GetBootCurrent();
            ushort nextEntryId = GetBootNext();

            using (BinaryReader reader = new BinaryReader(new MemoryStream(buffer, 0, (int)length))) {
                while (reader.BaseStream.Position != reader.BaseStream.Length) {
                    ushort optionId = reader.ReadUInt16();
                    EFI_LOAD_OPTION LoadOption = GetLoadOption(optionId);
                    entries.Add(new BootEntry() {
                        Id = optionId,
                        LoadOption = LoadOption,
                        IsCurrent = currentEntryId == optionId,
                        IsBootNext = nextEntryId == optionId
                    });
                }
            }
            return entries;
        }

        public static EFI_LOAD_OPTION GetLoadOption(ushort optionId) {
            byte[] buffer = null;
            uint length = ReadVariable(string.Format(LOAD_OPTION_FORMAT, optionId), ref buffer);
            return EFI_LOAD_OPTION.ConvertOption(buffer, (int)length);
        }

        public static ushort GetBootCurrent() {
            byte[] buffer = null;
            uint length = ReadVariable(EFI_BOOT_CURRENT, ref buffer);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public static ushort GetBootNext() {
            byte[] buffer = null;
            uint length = ReadVariable(EFI_BOOT_NEXT, ref buffer);
            return BitConverter.ToUInt16(buffer, 0);
        }

        public static bool SetVariable(string name, byte[] buffer) {
            bool result = NativeMethods.SetFirmwareEnvironmentVariable(name, EFI_GLOBAL_VARIABLE, buffer, (uint)buffer.Length);
            if (result) {
                return result;
            } else {
                throw new InvalidOperationException("Unable to set variable");
            }
        }

        public static bool SetBootNext(ushort entryId) {
            return SetVariable(EFI_BOOT_NEXT, BitConverter.GetBytes(entryId));
        }

        public static bool SetBootNext(BootEntry entry) {
            return SetBootNext(entry.Id);
        }
    }
}