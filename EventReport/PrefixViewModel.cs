using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Report
{
    internal class PrefixViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _Value;

        public string Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
                OnPropertyChanged();
            }
        }

        public PrefixViewModel()
        {
        }

        public PrefixViewModel(string value)
        {
            Value = value;
        }
    }
}
