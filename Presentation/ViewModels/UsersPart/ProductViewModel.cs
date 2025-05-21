using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Infrastructure.Data.Repositories;
using course_oop.Migrations;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.Views.User;

namespace course_oop.Presentation.ViewModels.UsersPart
{
    class ProductViewModel : ValidatedViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }
        public ICommand BackCommand { get; }

        private readonly Product _product;
        private readonly User _user;
        private readonly CardViewModel _cardViewModel;
        private readonly Shop _shop;
        private string _description;
        private int _mark = 1;
        private bool _isMarkSey;
        private string _basketText;
        private Core.Entities.Order? _order;

        private double _imageWidth;

        public string Name
        {
            get => _product.Name;
        }

        public string Description
        {
            get => _product.Description;
        }

        public string Price
        {
            get => string.Format("{0:C2}", _product.Price);
        }

        public string Weight
        {
            get => $"{_product.Weight} кг";
        }

        public string Count
        {
            get => _product.Count.ToString();
        }

        public string ShopName
        {
            get => _shop.Name;
        }

        public ObservableCollection<Review> Rewiews { get; }

        public ICommand SortByDateCommand { get; }

        public ICommand SortByMarkCommand { get; }

        public double ImageWidth
        {
            get => _imageWidth;
            set => SetValue(ref _imageWidth, value);
        }

        public ObservableCollection<ProductImage> Images { get; }

        [Required]
        [MaxLength(200, ErrorMessage = "Максимум 200 символов")]
        public string MarkDescription
        {
            get => _description;
            set
            {
                SetValue(ref _description, value);
                Validate(MarkDescription);
                _validFields[0] = true;
                if (!_isMarkSey) SetValidationResults(false, nameof(MarkDescription), ["Выберете оценку"]);
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public int Mark
        {
            get => _mark;
            set
            {
                SetValue(ref _mark, value);
                _validFields[1] = true;
                _isMarkSey = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public ICommand SetMark { get; }

        public ICommand AddReview { get; }

        public ICommand AddToBasket { get; }

        public ICommand Buy { get; }

        public string BasketText
        {
            get => _basketText;
            set => SetValue(ref _basketText, value);
        }

        private bool _btnBlock;

        public bool BtnBlock
        {
            get => _btnBlock;
            set
            {
                SetValue(ref _btnBlock, value);
            }
        }

        public ProductViewModel(CardViewModel cardViewModel, User user)
        {
            _product = cardViewModel._product;
            _cardViewModel = cardViewModel;
            _user = user;

            _description = string.Empty;
            using Repo repo = new();
            _shop = repo.GetShops().Cast<Shop>().Where(s => s.Id == _product.ShopId).First();
            Images = new(cardViewModel.Images);
            BackCommand = new Command(() => Navigator!.Navigate(new CatalogPage(_user)));

            using AppContext context = new();
            Rewiews = new(context.Rewiews.Where(r => r.ProductId == _product.Id));
            _order = context.Orders
                .Where(o => o.UserId == _user.Id && o.ProductId == _product.Id && o.Status == OrderStatus.InCart)
                .FirstOrDefault();

            if(_product.Count > 0) BtnBlock = true;

            if (_order == null) _basketText = "В корзину";
            else _basketText = "Из корзины";

            _validFields = new bool[2];

            SetMark = new Command(() =>
            {
                _isMarkSey = true;
                _validFields[1] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            });

            AddReview = new Command(() =>
            {
                using AppContext context = new();
                Review review = new()
                {
                    Content = MarkDescription,
                    UserName = _user.FirstName + " " + _user.Name,
                    ProductId = _product.Id,
                    CreatedAt = DateTime.Now,
                    Rating = Mark
                };

                context.Rewiews.Add(review);
                context.Products.Find(_product.Id)!.Mark = context.Rewiews.Average(r => r.Rating);
                _cardViewModel._product = context.Products.Find(_product.Id)!;
                context.SaveChanges();

                Navigator!.Navigate(new ProductPage(_user, _cardViewModel));
            });

            SortByDateCommand = new Command(() =>
            {
                var res = Rewiews.OrderByDescending(r => r.CreatedAt).ToList();
                Rewiews.Clear();
                res.ForEach(r => Rewiews.Add(r));
            });

            SortByMarkCommand = new Command(() =>
            {
                var res = Rewiews.OrderByDescending(r => r.Rating).ToList();
                Rewiews.Clear();
                res.ForEach(r => Rewiews.Add(r));
            });

            AddToBasket = new Command(() =>
            {
                using AppContext context = new();

                if (_order == null)
                {
                    Core.Entities.Order order = new()
                    {
                        CreatedDate = DateTime.Now,
                        UserId = _user.Id,
                        ProductId = _product.Id,
                        ShopId = _shop.Id,
                        Status = OrderStatus.InCart,
                    };


                    context.Orders.Add(order);
                    BasketText = "Из корзины";

                    _order = order;
                }
                else
                {
                    context.Remove(_order);
                    BasketText = "В корзину";
                    _order = null;
                }

                context.SaveChanges();
            });

            Buy = new Command(() =>
            {
                using AppContext context = new();

                Core.Entities.Order order = new()
                {
                    CreatedDate = DateTime.Now,
                    UserId = _user.Id,
                    ProductId = _product.Id,
                    ShopId = _shop.Id,
                    Status = OrderStatus.InCart,
                };

                context.Orders.Add(order);

                context.SaveChanges();

                Navigator!.Navigate(new BasketPage(_user, order));
            });
        }
    }
}