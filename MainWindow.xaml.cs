using System.Windows;
using EFIReboot.Model;
using EFIReboot.WinAPI;
using MahApps.Metro.Controls;

namespace EFIReboot {

    public partial class MainWindow : MetroWindow {
        private MainViewModel ViewModel;

        public MainWindow() {
            InitializeComponent();
            ViewModel = new MainViewModel(EFIHelper.GetEntries());
            ViewModel.RebootClick += OnRebootClick;
            ViewModel.BootNextClick += OnBootNextClick;
            BootEntries.DataContext = ViewModel;
        }

        private void OnBootNextClick(object sender, BootEntryEventArgs e) {
            EFIHelper.SetBootNext(e.BootEntry);
            ViewModel.MarkAsNext(e.BootEntry);
        }

        private void OnRebootClick(object sender, BootEntryEventArgs e) {
            string message = string.Format("Are you sure want to reboot to {0} right now?", e.BootEntry.Name);
            if (MessageBoxResult.Yes == MessageBox.Show(message, "Reboot confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)) {
                OnBootNextClick(sender, e);
                if (!NativeMethods.ExitWindowsEx(ExitWindows.Reboot,
                        ShutdownReason.MajorApplication |
                ShutdownReason.MinorInstallation |
                ShutdownReason.FlagPlanned)) {
                    MessageBox.Show("Unable to reboot your computer", "Reboot failure", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}