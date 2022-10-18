using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Report
{
    internal class CategoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

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

        private ObservableCollection<PrefixViewModel> _Prefixes = new ObservableCollection<PrefixViewModel>();

        public ObservableCollection<PrefixViewModel> Prefixes
        {
            get { return _Prefixes; }
            set { _Prefixes = value; OnPropertyChanged(); }
        }

        public CategoryViewModel()
        {
        }

        public CategoryViewModel(string name)
        {
            Name = name;
        }

        public CategoryViewModel(Category category)
        {
            Name = category.Key;
            foreach (string item in category.Prefixes)
            {
                Prefixes.Add(new PrefixViewModel(item));
            }
        }
    }
}
