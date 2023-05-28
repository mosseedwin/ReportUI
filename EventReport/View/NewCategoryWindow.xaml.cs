using System.Windows;

namespace Report
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewCategoryWindow : Window
    {
        public string Category { get; set; }

        public string Subcategory { get; set; }

        internal NewCategoryWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
