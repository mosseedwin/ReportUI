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
            if (OpenFileDialog(MainViewModel.EventsFile, out string fileName))
            {
                MainViewModel.EventsFile = fileName;
            }
        }

        private void IncidentsFileSearcher(object sender, RoutedEventArgs e)
        {
            if (OpenFileDialog(MainViewModel.IncidentsFile, out string fileName))
            {
                MainViewModel.IncidentsFile = fileName;
            }
        }

        private void ChangesFileSearcher(object sender, RoutedEventArgs e)
        {
            if (OpenFileDialog(MainViewModel.ChangesFile, out string fileName))
            {
                MainViewModel.ChangesFile = fileName;
            }
        }

        private void AvailabilityFileSearcher(object sender, RoutedEventArgs e)
        {
            if (OpenFileDialog(MainViewModel.AvailabilityFile, out string fileName))
            {
                MainViewModel.AvailabilityFile = fileName;
            }
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

        private bool OpenFileDialog(string reference, out string fileName)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Archivo csv|*.csv";
            try
            {
                if (!string.IsNullOrWhiteSpace(reference))
                {
                    string directoryName = Path.GetDirectoryName(reference);
                    if (Directory.Exists(directoryName))
                    {
                        dialog.InitialDirectory = directoryName;
                    }
                }
            }
            catch
            {
            }
            if (dialog.ShowDialog() == true)
            {
                if (File.Exists(dialog.FileName))
                {
                    fileName = dialog.FileName;
                    return true;
                }
            }
            fileName = null;
            return false;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel.Start(Countries, Categories);
        }
    }
}
