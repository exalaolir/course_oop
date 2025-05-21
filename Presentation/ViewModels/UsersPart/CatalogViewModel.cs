using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Infrastructure.Data.Repositories;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.Views.User;

namespace course_oop.Presentation.ViewModels.UsersPart
{
    public sealed class CardViewModel(Product product, CatalogViewModel? action, IEnumerable<ProductImage> images)
        : ViewModel
    {
        public Product _product = product;
        private readonly CatalogViewModel? _action = action;

        public string Mark
        {
            get => _product.Mark.ToString("0.00");
        }

        public static string Star
        {
            get => "\xE735";
        }

        public decimal Price
        {
            get => _product.Price;
        }

        public string PriceStr
        {
            get => string.Format("{0:C2}", _product.Price);
        }

        public string Name
        {
            get => _product.Name;
            set => _product.Name = value;
        }

        public ObservableCollection<ProductImage> Images = new(images);

        public string Path
        {
            get => Images[0].Path;
        }

        public int Category => _product.CategotyId;

        ~CardViewModel () => Debug.WriteLine("CardViewModel finalized");

        public ICommand? NavToProduct => new Command<CardViewModel>(vm => _action!.Navigate(vm));
    }

    public sealed class CatalogViewModel : ViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }
        public ObservableCollection<CardViewModel> Products { get; set; }

        public ObservableCollection<CardViewModel> AllProducts { get; set; }

        public ObservableCollection<Category> Categories { get; set; }

        public ICommand SortByNameCommand { get; }
        public ICommand SortByPriceCommand { get; }
        public ICommand SortByRatingAscCommand { get; }
        public ICommand SortByRatingDescCommand { get; }

        public ICommand SearchCommand { get; }

        private readonly User _user;
        private Category _category;
        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set => SetValue(ref _searchText, value);
        }

        public Category Category
        {
            get => _category;
            set
            {
                SetValue(ref _category, value);
                if (value.Name == "Все")
                {
                    Products = new(AllProducts);
                }
                else
                {
                    Products = new(AllProducts.Where(c => c.Category == value.Id));
                }

                OnPropertyChanged(nameof(Products));
            }
        }

        public CatalogViewModel(User user)
        {
            _searchText = string.Empty;
            _user = user;
            using Repo repo = new();
            using AppContext context = new();
            var images = context.ProductImages.ToList();
            Products = new(
                repo.GetAllProducts()
                    .Cast<Product>()
                    .Select(e => new CardViewModel(e, this, images.Where(i => i.ProductId == e.Id)))
            );
            _category = new Category { Name = "Все" };
            Categories = [_category];
            AllProducts = [];

            var categories = repo.GetCategories().Cast<Category>().Where(c => c.ParentId != null);

            foreach (var item in categories)
            {
                Categories.Add(item);
            }

            foreach (var item in Products)
            {
                AllProducts.Add(item);
            }

            SortByNameCommand = new Command(SortByName);
            SortByPriceCommand = new Command(SortByPrice);
            SortByRatingAscCommand = new Command(SortByRatingAsc);
            SortByRatingDescCommand = new Command(SortByRatingDesc);

            SearchCommand = new Command(() =>
            {
                if (_searchText == string.Empty)
                {
                    Products = new(AllProducts);
                }
                else
                {
                    Products = new(AllProducts.Where(p =>
                        Regex.IsMatch(p.Name, $"^{Regex.Escape(_searchText)}", RegexOptions.IgnoreCase) &&
                        (p.Category == Category.Id || Category.Name == "Все")));
                }

                OnPropertyChanged(nameof(Products));
            });
        }

        ~CatalogViewModel () => Debug.WriteLine("CatalogViewModel finalized");

        public void Navigate(CardViewModel product)
        {
            Navigator!.Navigate(new ProductPage(_user, product));
        }

        private void SortByName()
        {
            var sorted = Products.OrderBy(p => p.Name).ToList();
            Products = new ObservableCollection<CardViewModel>(sorted);
            OnPropertyChanged(nameof(Products));
        }

        private void SortByPrice()
        {
            var sorted = Products.OrderBy(p => p.Price).ToList();
            Products = new ObservableCollection<CardViewModel>(sorted);
            OnPropertyChanged(nameof(Products));
        }

        private void SortByRatingAsc()
        {
            var sorted = Products.OrderBy(p => p.Mark).ToList();
            Products = new ObservableCollection<CardViewModel>(sorted);
            OnPropertyChanged(nameof(Products));
        }

        private void SortByRatingDesc()
        {
            var sorted = Products.OrderByDescending(p => p.Mark).ToList();
            Products = new ObservableCollection<CardViewModel>(sorted);
            OnPropertyChanged(nameof(Products));
        }
    }
}