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
using System.Windows;
using System.Windows.Navigation;
using MahApps.Metro.Controls;

namespace EFIReboot {

    public partial class AboutWindow : MetroWindow {

        public AboutWindow(MetroWindow owner) : this() {
            this.Owner = owner;
        }

        public AboutWindow() {
            InitializeComponent();
        }

        private void UpdataVersionText() {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            VersionBlock.Text = string.Format("Version: {1}.{2} (build {3})", version.Major, version.Minor, version.Build);
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e) {
            try {
                System.Diagnostics.Process.Start(System.Web.HttpUtility.UrlDecode(e.Uri.AbsoluteUri));
            } catch (Exception ex) {
                MessageBox.Show("Unable to open URL: " + e.Uri.AbsoluteUri, "URL open error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}