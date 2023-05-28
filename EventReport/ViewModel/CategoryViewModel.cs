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
                OnPropertyChanged(nameof(FullName));
            }
        }

        private string _Subcategory;

        public string Subcategory
        {
            get { return _Subcategory; }
            set
            {
                _Subcategory = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Subcategory))
                {
                    return Name;
                }
                else
                {
                    return Name + " : " + Subcategory;
                }
            }
            set
            {
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

        public CategoryViewModel(string category, string subcategory)
        {
            Name = category;
            Subcategory = subcategory;
        }

        public CategoryViewModel(Category category)
        {
            Name = category.Name;
            Subcategory = category.Subcategory;
            foreach (string item in category.Prefixes)
            {
                Prefixes.Add(new PrefixViewModel(item));
            }
        }
    }
}
