using System.Collections.Generic;
using System.IO;

namespace EFIReboot.EFI {

    /// <summary>
    /// An UEFI EFI_DEVICE_PATH_PROTOCOL (9.2-9.3 sections of UEFI Specification)
    /// </summary>
    public struct EFI_DEVICE_PATH_PROTOCOL {

        public enum DeviceType : byte {
            HW          = 0x01,
            ACPI        = 0x02,
            MSG         = 0x03,
            MEDIA       = 0x04,
            BIOS        = 0x05,
            END         = 0x7F
        }

        private static byte END_ENTIRE_DEVICE_PATH_SUB_TYPE = 0xFF;

        DeviceType Type {
            get;
            set;
        }

        byte SubType {
            get;
            set;
        }

        ushort Length {
            get;
            set;
        }

        byte[] Data {
            get;
            set;
        }

        public static List<EFI_DEVICE_PATH_PROTOCOL> ReadList(BinaryReader reader) {
            List<EFI_DEVICE_PATH_PROTOCOL> protocols = new List<EFI_DEVICE_PATH_PROTOCOL>();
            while (true) {
                EFI_DEVICE_PATH_PROTOCOL protocol = new EFI_DEVICE_PATH_PROTOCOL();
                protocol.Type = (DeviceType) reader.ReadByte();
                protocol.SubType = reader.ReadByte();
                protocol.Length = reader.ReadUInt16();
                protocol.Data = reader.ReadBytes(protocol.Length - 4);
                protocols.Add(protocol);
                if (protocol.Type == DeviceType.END && protocol.SubType == END_ENTIRE_DEVICE_PATH_SUB_TYPE) {
                    break;
                }
            }
            return protocols;
        }
    }
}