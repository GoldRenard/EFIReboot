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

using System.IO;
using System.Windows;
using System.Windows.Media;
using EFIReboot.Model;
using EFIReboot.WinAPI;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace EFIReboot {

    public partial class MainWindow : MetroWindow {
        private int HEADER_HEIGHT_CONST = 59;
        private int ITEM_HEIGHT_CONST = 35;

        private MainViewModel ViewModel;

        public MainWindow() {
            InitializeComponent();
            ViewModel = new MainViewModel(EFIHelper.GetEntries());
            ViewModel.RebootClick += OnRebootClick;
            ViewModel.BootNextClick += OnBootNextClick;
            ViewModel.CreateShortcutClick += OnCreateShortcut;
            BootEntries.DataContext = ViewModel;
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e) {
            if (ViewModel.Items.Count <= 10) {
                this.Height = ViewModel.Items.Count * ITEM_HEIGHT_CONST + HEADER_HEIGHT_CONST;
            }
        }

        private void OnBootNextClick(object sender, BootEntryEventArgs e) {
            EFIHelper.SetBootNext(e.BootEntry);
            ViewModel.MarkAsNext(e.BootEntry);
        }

        public void OnRebootClick(object sender, BootEntryEventArgs e) {
            App.DoReboot(e.BootEntry);
        }

        public void OnCreateShortcut(object sender, BootEntryEventArgs e) {
            string message = string.Format("Do you want to create shortcut to {0} WITHOUT reboot confirmation?", e.BootEntry.Name);
            bool hasConfirmation = MessageBoxResult.Yes != MessageBox.Show(message, "Reboot confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = string.Join(string.Empty, e.BootEntry.Name.Split(Path.GetInvalidFileNameChars()));
            saveFileDialog.Filter = "Shortcut (*.lnk)|*.lnk";
            if (saveFileDialog.ShowDialog() == true) {
                using (ShellLink shortcut = new ShellLink()) {
                    shortcut.Target = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    shortcut.WorkingDirectory = Path.GetDirectoryName(shortcut.Target);
                    shortcut.Description = string.Format("Boot to {0}", e.BootEntry.Name);
                    shortcut.DisplayMode = ShellLink.LinkDisplayMode.edmNormal;
                    shortcut.Arguments = string.Format(hasConfirmation ? "{0}" : "{0} quiet", e.BootEntry.Id);
                    shortcut.Save(saveFileDialog.FileName);
                }
            }
        }

        private void About_Click(object sender, RoutedEventArgs e) {
            new AboutWindow(this).ShowDialog();
        }

        public static T FindVisualChild<T>(DependencyObject current) where T : DependencyObject {
            if (current == null) return null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(current);
            for (int i = 0; i < childrenCount; i++) {
                DependencyObject child = VisualTreeHelper.GetChild(current, i);
                if (child is T) return (T)child;
                T result = FindVisualChild<T>(child);
                if (result != null) return result;
            }
            return null;
        }
    }
}