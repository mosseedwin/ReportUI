using System.Windows;

namespace Report
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class NewCountryWindow : Window
    {
        public string InstanceName { get; set; }

        internal NewCountryWindow()
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
