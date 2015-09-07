using System;
using System.IO;
using System.Text;

namespace EFIReboot.EFI {

    /// <summary>
    /// An UEFI Load Option (3.1.3 section of UEFI Specification)
    /// </summary>
    public struct EFI_LOAD_OPTION {
        public const UInt32 LOAD_OPTION_ACTIVE = 0x00000001;
        public const UInt32 LOAD_OPTION_FORCE_RECONNECT = 0x00000002;
        public const UInt32 LOAD_OPTION_HIDDEN = 0x00000008;
        public const UInt32 LOAD_OPTION_CATEGORY = 0x00001F00;
        public const UInt32 LOAD_OPTION_CATEGORY_BOOT = 0x00000000;
        public const UInt32 LOAD_OPTION_CATEGORY_APP = 0x00000100;

        public UInt32 Attributes {
            get;
            set;
        }

        public UInt16 FilePathListLength {
            get;
            set;
        }

        public string Description {
            get;
            set;
        }

        public byte[] FilePathList {
            get;
            set;
        }

        /*public EFI_DEVICE_PATH_PROTOCOL[] FilePathList {
            get;
            set;
        }*/

        public byte[] OptionalData {
            get;
            set;
        }

        public static EFI_LOAD_OPTION ConvertOption(byte[] data, int lenght) {
            EFI_LOAD_OPTION LoadOption = new EFI_LOAD_OPTION();
            using (BinaryReader reader = new BinaryReader(new MemoryStream(data, 0, lenght))) {
                LoadOption.Attributes = reader.ReadUInt32();
                LoadOption.FilePathListLength = reader.ReadUInt16();
                StringBuilder builder = new StringBuilder();
                while (true) {
                    ushort chr = reader.ReadUInt16();
                    if (chr == 0) {
                        break;
                    }
                    builder.Append(Convert.ToChar(chr));
                }
                LoadOption.Description = builder.ToString();
                LoadOption.FilePathList = reader.ReadBytes(LoadOption.FilePathListLength);
                LoadOption.OptionalData = reader.ReadBytes((int)(lenght - reader.BaseStream.Position));
            }
            return LoadOption;
        }
    }
}