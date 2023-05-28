using System.Windows;

namespace Report
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CountriesWindow : Window
    {
        private CountriesViewModel ViewModel { get; }

        internal CountriesWindow(CountriesViewModel dataContext)
        {
            InitializeComponent();
            ViewModel = dataContext;
            DataContext = dataContext;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            NewCountryWindow countryWindow = new NewCountryWindow();
            if (countryWindow.ShowDialog() == true)
            {
                Country country = new Country() { Name = countryWindow.InstanceName };
                ViewModel.Items.Add(new CountryViewModel(country));
            }
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            if (CountriesUI.SelectedItem is CountryViewModel item)
            {
                ViewModel.Items.Remove(item);
            }
        }

        private void AddPrefix(object sender, RoutedEventArgs e)
        {
            if (CountriesUI.SelectedItem is CountryViewModel item)
            {
                PrefixViewModel prefixViewModel = new PrefixViewModel();
                item.Prefixes.Add(prefixViewModel);
            }
        }

        private void RemovePrefix(object sender, RoutedEventArgs e)
        {
            if (CountriesUI.SelectedItem is CountryViewModel item)
            {
                if (PrefixesUI.SelectedItem is PrefixViewModel prefixViewModel)
                {
                    item.Prefixes.Remove(prefixViewModel);
                }
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            ViewModel.Save();
            DialogResult = true;
            Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
