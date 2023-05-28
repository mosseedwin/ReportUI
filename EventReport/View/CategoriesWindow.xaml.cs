using System.Windows;

namespace Report
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class CategoriesWindow : Window
    {
        private CategoriesViewModel ViewModel { get; }

        internal CategoriesWindow(CategoriesViewModel dataContext)
        {
            InitializeComponent();
            ViewModel = dataContext;
            DataContext = dataContext;
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            NewCategoryWindow window = new NewCategoryWindow();
            if (window.ShowDialog() == true)
            {
                ViewModel.Items.Add(new CategoryViewModel(window.Category, window.Subcategory));
            }
        }

        private void Remove(object sender, RoutedEventArgs e)
        {
            if (CountriesUI.SelectedItem is CategoryViewModel item)
            {
                ViewModel.Items.Remove(item);
            }
        }

        private void AddPrefix(object sender, RoutedEventArgs e)
        {
            if (CountriesUI.SelectedItem is CategoryViewModel item)
            {
                PrefixViewModel prefixViewModel = new PrefixViewModel();
                item.Prefixes.Add(prefixViewModel);
            }
        }

        private void RemovePrefix(object sender, RoutedEventArgs e)
        {
            if (CountriesUI.SelectedItem is CategoryViewModel item)
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
