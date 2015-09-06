using System.Collections.Generic;
using System.ComponentModel;

namespace EFIReboot.Model {

    internal class BootEntry : INotifyPropertyChanged {
        public const string INTERNAL_FORMAT = "Boot{0:X4}";

        public event PropertyChangedEventHandler PropertyChanged;

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

        private string _Name;

        public string Name {
            get {
                return _Name;
            }
            set {
                if (_Name != value) {
                    _Name = value;
                    NotifyPropertyChanged("Name");
                }
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