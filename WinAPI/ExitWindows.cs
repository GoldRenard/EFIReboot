using System;

namespace EFIReboot.WinAPI {

    [Flags]
    internal enum ExitWindows : uint {
        LogOff = 0x00,
        ShutDown = 0x01,
        Reboot = 0x02,
        PowerOff = 0x08,
        RestartApps = 0x40,
        Force = 0x04,
        ForceIfHung = 0x10,
    }
}