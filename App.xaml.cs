using System;
using System.Windows;
using EFIReboot.WinAPI;

namespace EFIReboot {

    public partial class App : Application {

        private void Application_Startup(object sender, StartupEventArgs e) {
            if (!EFIHelper.IsSupported()) {
                MessageBox.Show("Only UEFI platform is supported", "Not supported", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }
            try {
                PrivelegeHelper.ObtainSystemPrivileges();
            } catch (InvalidOperationException ex) {
                MessageBox.Show(string.Format("Unable to obtain system privileges: {0}", ex.Message), "Application error", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }
        }
    }
}