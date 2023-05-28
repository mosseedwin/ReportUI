using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Report
{
    internal class CountryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Country Country { get; }

        private string _Name;

        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged();
            }
        }

        private byte _Wave;

        public byte Wave
        {
            get { return _Wave; }
            set { _Wave = value; OnPropertyChanged(); }
        }

        private byte _Servers;

        public byte Servers
        {
            get { return _Servers; }
            set { _Servers = value; OnPropertyChanged(); }
        }

        private byte _Maximum;

        public byte Maximum
        {
            get { return _Maximum; }
            set { _Maximum = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PrefixViewModel> _Prefixes = new ObservableCollection<PrefixViewModel>();

        public ObservableCollection<PrefixViewModel> Prefixes
        {
            get { return _Prefixes; }
            set { _Prefixes = value; OnPropertyChanged(); }
        }

        public CountryViewModel()
        {
        }

        public CountryViewModel(Country country)
        {
            Country = country;
            Name = country.Name;
            Wave = country.Wave;
            Servers = country.Servers;
            Maximum = country.Maximum;
            foreach (string p in country.Prefixes)
            {
                Prefixes.Add(new PrefixViewModel(p));
            }
        }
    }
}
