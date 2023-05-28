using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Report
{
    internal class CountriesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<CountryViewModel> _Items = new ObservableCollection<CountryViewModel>();

        public ObservableCollection<CountryViewModel> Items
        {
            get { return _Items; }
            set { _Items = value; OnPropertyChanged(); }
        }

        private CountryViewModel _Selected;
        public CountryViewModel Selected
        {
            get { return _Selected; }
            set { _Selected = value; OnPropertyChanged(); }
        }

        public CountriesViewModel(Countries countries)
        {
            foreach (Country country in countries)
            {
                Items.Add(new CountryViewModel(country));
            }
        }

        public void Remove(CountryViewModel item)
        {
            Items.Remove(item);
        }

        internal void Save()
        {
            Countries countries = new Countries();
            foreach (CountryViewModel countryViewModel in Items)
            {
                Country country = new Country()
                {
                    Name = countryViewModel.Name,
                    Wave = countryViewModel.Wave,
                    Servers = countryViewModel.Servers,
                    Maximum = countryViewModel.Maximum,
                };
                foreach (PrefixViewModel prefixViewModel in countryViewModel.Prefixes)
                {
                    if (!string.IsNullOrWhiteSpace(prefixViewModel.Value))
                    {
                        country.Prefixes.Add(prefixViewModel.Value);
                    }
                }
                countries.Add(country);
            }
            countries.Save();
        }
    }
}
