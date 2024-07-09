using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DataManager
{
    internal class ImportViewModel : INotifyPropertyChanged
    {
        private string _statusMessage;
        private int _progress;
        private Visibility _statusBarVisibility;

        public ImportViewModel()
        {
            _statusBarVisibility = Visibility.Collapsed;
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
        public Visibility StatusBarVisibility
        {
            get => _statusBarVisibility;
            set
            {
                _statusBarVisibility = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
