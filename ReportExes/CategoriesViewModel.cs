using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Report
{
    internal class CategoriesViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<CategoryViewModel> _Items = new ObservableCollection<CategoryViewModel>();

        public ObservableCollection<CategoryViewModel> Items
        {
            get { return _Items; }
            set { _Items = value; OnPropertyChanged(); }
        }

        private CategoryViewModel _Selected;
        public CategoryViewModel Selected
        {
            get { return _Selected; }
            set { _Selected = value; OnPropertyChanged(); }
        }

        public CategoriesViewModel(Categories categories)
        {
            foreach (Category category in categories)
            {
                Items.Add(new CategoryViewModel(category));
            }
        }

        public void Remove(CategoryViewModel item)
        {
            Items.Remove(item);
        }

        internal void Save()
        {
            Categories categories = new Categories();
            foreach (CategoryViewModel categoryViewModel in Items)
            {
                Category category = new Category()
                {
                    Key = categoryViewModel.Name,
                };
                foreach (PrefixViewModel prefixViewModel in categoryViewModel.Prefixes)
                {
                    if (!string.IsNullOrWhiteSpace(prefixViewModel.Value))
                    {
                        category.Prefixes.Add(prefixViewModel.Value);
                    }
                }
                categories.Add(category);
            }
            categories.Save();
        }
    }
}
