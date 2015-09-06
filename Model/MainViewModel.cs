using System.Collections.Generic;

namespace EFIReboot.Model {

    internal class MainViewModel {

        public MainViewModel(List<BootEntry> Items) {
            this.RebootCommand = new DelegateCommand<BootEntry>(m => OnRebootClick(m), m => m != null);
            this.MouseDoubleClickCommand = new DelegateCommand<BootEntry>(m => OnRebootClick(m), m => m != null);
            this.BootNextCommand = new DelegateCommand<BootEntry>(m => OnNextBootClick(m), m => m != null && !m.IsBootNext);
            foreach (BootEntry entry in Items) {
                entry.ParentViewModel = this;
            }
            this.Items = Items;
        }

        public event BootEntryEventHandler RebootClick;

        public event BootEntryEventHandler BootNextClick;

        public DelegateCommand<BootEntry> RebootCommand {
            get; set;
        }

        public DelegateCommand<BootEntry> BootNextCommand {
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

        public void MarkAsNext(BootEntry nextEntry) {
            foreach (BootEntry entry in Items) {
                entry.IsBootNext = false;
            }
            nextEntry.IsBootNext = true;
        }
    }
}