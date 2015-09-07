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
using System.ComponentModel;
using EFIReboot.EFI;

namespace EFIReboot.Model {

    public class BootEntry : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        private EFI_LOAD_OPTION _LoadOption;

        public EFI_LOAD_OPTION LoadOption {
            get {
                return _LoadOption;
            }
            set {
                if (!_LoadOption.Equals(value)) {
                    _LoadOption = value;
                    NotifyPropertyChanged("LoadOption");
                    NotifyPropertyChanged("Name");
                }
            }
        }

        private ushort _Id = 0;

        public ushort Id {
            get {
                return _Id;
            }
            set {
                if (_Id != value) {
                    _Id = value;
                    NotifyPropertyChanged("Id");
                    NotifyPropertyChanged("InternalName");
                }
            }
        }

        public string Name {
            get {
                return LoadOption.Description;
            }
        }

        public string Status {
            get {
                List<string> statuses = new List<string>();
                if (IsCurrent) {
                    statuses.Add("Current");
                }
                if (IsBootNext) {
                    statuses.Add("Next boot");
                }
                return string.Join(", ", statuses);
            }
        }

        public bool IsCurrent {
            get;
            set;
        }

        private bool _IsBootNext;

        public bool IsBootNext {
            get {
                return _IsBootNext;
            }
            set {
                if (_IsBootNext != value) {
                    _IsBootNext = value;
                    NotifyPropertyChanged("IsBootNext");
                    NotifyPropertyChanged("Status");
                }
            }
        }

        public string InternalName {
            get {
                return string.Format("Boot{0:X4}", Id);
            }
        }

        public MainViewModel ParentViewModel {
            get; set;
        }

        protected void NotifyPropertyChanged(string propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}