using Microsoft.Win32;
using System.IO;
using System.Windows;
using Path = System.IO.Path;

namespace Report
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Countries Countries { get; set; } = new Countries();

        private Categories Categories { get; set; } = new Categories();

        private MainViewModel MainViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();
            MainViewModel = new MainViewModel();
            DataContext = MainViewModel;
            LoadCountries();
            LoadCategories();
        }
        private void LoadCountries()
        {
            Countries = Countries.Load();
        }

        private void LoadCategories()
        {
            Categories = Categories.Load();
        }

        private void Countries_Click(object sender, RoutedEventArgs e)
        {
            CountriesWindow countriesWindow = new CountriesWindow(new CountriesViewModel(Countries));
            if (countriesWindow.ShowDialog() == true)
            {
                LoadCountries();
            }
        }

        private void Categories_Click(object sender, RoutedEventArgs e)
        {
            CategoriesWindow countriesWindow = new CategoriesWindow(new CategoriesViewModel(Categories));
            if (countriesWindow.ShowDialog() == true)
            {
                LoadCategories();
            }
        }

        private void EventsFileSearcher(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Archivo csv|*.csv";
            try
            {
                if (!string.IsNullOrWhiteSpace(MainViewModel.EventsFile))
                {
                    string directoryName = Path.GetDirectoryName(MainViewModel.EventsFile);
                    if (Directory.Exists(directoryName))
                    {
                        dialog.InitialDirectory = directoryName;
                    }
                }
            }
            catch
            {
            }
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            if (!File.Exists(dialog.FileName))
            {
                return;
            }
            MainViewModel.EventsFile = dialog.FileName;
        }

        private void OutputFileSearcher(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Archivo Excel|*.xlsx";
            try
            {
                if (!string.IsNullOrWhiteSpace(MainViewModel.OutputFile))
                {
                    string directoryName = Path.GetDirectoryName(MainViewModel.OutputFile);
                    if (Directory.Exists(directoryName))
                    {
                        dialog.InitialDirectory = directoryName;
                    }
                }
            }
            catch
            {
            }
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            MainViewModel.OutputFile = dialog.FileName;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Start(Countries, Categories);
        }
    }
}
