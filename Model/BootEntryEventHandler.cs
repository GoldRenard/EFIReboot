using System;

namespace EFIReboot.Model {

    /// <summary>
    /// Login complete event handler
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    internal delegate void BootEntryEventHandler(object sender, BootEntryEventArgs e);

    /// <summary>
    /// Boot Entry event arguments
    /// </summary>
    internal class BootEntryEventArgs : EventArgs {

        /// <summary>
        /// Gets boot entry
        /// </summary>
        public BootEntry BootEntry {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="BootEntryEventArgs"/> for specified <see cref="BootEntry"/>.
        /// </summary>
        /// <param name="BootEntry">BootEntry</param>
        public BootEntryEventArgs(BootEntry BootEntry) {
            this.BootEntry = BootEntry;
        }
    }
}