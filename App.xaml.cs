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
using System.Linq;
using System.Reflection;
using System.Windows;
using EFIReboot.EFI;
using EFIReboot.Model;
using EFIReboot.WinAPI;
using MahApps.Metro;

namespace EFIReboot {

    public partial class App : Application {

        public App() {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += (s, a) => {
                string resourceName = "EFIReboot." + new AssemblyName(a.Name).Name + ".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
                    byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }

        private void Application_Startup(object sender, StartupEventArgs e) {
            if (!EFIEnvironment.IsSupported()) {
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
            if (ProcessCommandLine()) {
                Shutdown();
                return;
            }

            ThemeManager.ChangeAppStyle(this, ThemeManager.GetAccent("Yellow"), ThemeManager.GetAppTheme("BaseDark"));
            new MainWindow().Show();
        }

        private bool ProcessCommandLine() {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1) {
                ushort entryId = 0;
                if (ushort.TryParse(args[1], out entryId)) {
                    BootEntry entry = EFIEnvironment.GetEntries().FirstOrDefault(e => e.Id == entryId);
                    if (entry != null) {
                        DoReboot(entry, !args.Contains("quiet"));
                        return true;
                    }
                }
            }
            return false;
        }

        public static void DoReboot(BootEntry entry, bool confirmation = true) {
            if (confirmation) {
                string message = string.Format("Are you sure want to reboot to {0} right now?", entry.Name);
                if (MessageBoxResult.Yes != MessageBox.Show(message, "Reboot confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)) {
                    return;
                }
            }
            EFIEnvironment.SetBootNext(entry);
            if (!NativeMethods.ExitWindowsEx(ExitWindows.Reboot,
                    ShutdownReason.MajorApplication |
            ShutdownReason.MinorInstallation |
            ShutdownReason.FlagPlanned)) {
                MessageBox.Show("Unable to reboot your computer", "Reboot failure", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowException(Exception e) {
            if (e == null) {
                Application.Current.Shutdown();
                return;
            }
            string errorMessage = string.Format("An application error occurred.\n\n{0} ({1}):\n{2}", e, e.Message, e.StackTrace);
            MessageBox.Show(errorMessage, "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            ShowException(e.ExceptionObject as Exception);
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            ShowException(e.Exception);
        }
    }
}