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

using System.Collections.Generic;

namespace EFIReboot.Model {

    public class MainViewModel {

        public MainViewModel(List<BootEntry> Items) {
            this.RebootCommand = new DelegateCommand<BootEntry>(m => OnRebootClick(m), m => m != null);
            this.MouseDoubleClickCommand = new DelegateCommand<BootEntry>(m => OnRebootClick(m), m => m != null);
            this.BootNextCommand = new DelegateCommand<BootEntry>(m => OnNextBootClick(m), m => m != null && !m.IsBootNext);
            this.CreateShortcutCommand = new DelegateCommand<BootEntry>(m => OnCreateShortcutClick(m), m => m != null);
            foreach (BootEntry entry in Items) {
                entry.ParentViewModel = this;
            }
            this.Items = Items;
        }

        public event BootEntryEventHandler RebootClick;

        public event BootEntryEventHandler BootNextClick;

        public event BootEntryEventHandler CreateShortcutClick;

        public DelegateCommand<BootEntry> RebootCommand {
            get; set;
        }

        public DelegateCommand<BootEntry> BootNextCommand {
            get; set;
        }

        public DelegateCommand<BootEntry> CreateShortcutCommand {
            get; set;
        }

        public DelegateCommand<BootEntry> MouseDoubleClickCommand {
            get; set;
        }

        public List<BootEntry> Items {
            get; set;
        }

        private void OnRebootClick(BootEntry entry) {
            if (RebootClick != null) {
                RebootClick(this, new BootEntryEventArgs(entry));
            }
        }

        private void OnNextBootClick(BootEntry entry) {
            if (BootNextClick != null) {
                BootNextClick(this, new BootEntryEventArgs(entry));
            }
        }

        private void OnCreateShortcutClick(BootEntry entry) {
            if (CreateShortcutClick != null) {
                CreateShortcutClick(this, new BootEntryEventArgs(entry));
            }
        }

        public void MarkAsNext(BootEntry nextEntry) {
            foreach (BootEntry entry in Items) {
                entry.IsBootNext = false;
            }
            nextEntry.IsBootNext = true;
        }
    }
}