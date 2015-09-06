namespace EFIReboot.WinAPI {

    internal enum FirmwareType : uint {
        FirmwareTypeUnknown = 0,
        FirmwareTypeBios = 1,
        FirmwareTypeUefi = 2,
        FirmwareTypeMax = 3
    };
}