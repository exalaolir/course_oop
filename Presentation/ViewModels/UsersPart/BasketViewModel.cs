using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Core.Services;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.Views.User;

namespace course_oop.Presentation.ViewModels.UsersPart
{
    internal sealed class BasketCardViewModel : ValidatedViewModel
    {
        private readonly Order _order;
        private readonly Product _product;
        private readonly INavigation? _navigator;
        private string _adress;

        public CardViewModel Card { get; }

        public string Name => _product.Name;
        public string Path { get; }
        public string Mark => $" Оценка: {_product.Mark.ToString("0.00")}";

        public string Price => string.Format("{0:C2}", _product.Price);

        [Required]
        public string Adress
        {
            get => _adress;
            set
            {
                SetValue(ref _adress, value);
                Validate(Adress);
                _validFields[0] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public ICommand Delete { get; }
        public ICommand Watch { get; }

        public ICommand Enter { get; }

        public Order Order => _order;

        public BasketCardViewModel(
            Order order,
            Product product,
            ICollection<ProductImage> productImages,
            INavigation? navigation,
            params Action<BasketCardViewModel>[] actions)
        {
            _order = order;
            _adress = string.Empty;
            _product = product;
            _navigator = navigation;
            Card = new CardViewModel(product, null, productImages);
            Path = productImages.First().Path;

            _validFields = new bool[2];

            if (_product.Count > 0)
            {
                _validFields[1] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }

            Delete = new Command<BasketCardViewModel>(item => actions[0].Invoke(item));
            Watch = new Command<BasketCardViewModel>(i => actions[1].Invoke(i));

            Enter = new Command<BasketCardViewModel>(async item =>
            {
                try
                {
                    var coords = await MapServise.GetCoordsByQuery(Adress);

                    using AppContext appContext = new();

                    var order = appContext.Orders.Find(_order.Id);
                    order!.Status = OrderStatus.Processing;
                    order!.DeliveryAddress = coords.adress;
                    order!.X = coords.x;
                    order!.Y = coords.y;
                    appContext.Products.Find(_product.Id)!.Count -= 1;
                    appContext.SaveChanges();

                    actions[2].Invoke(item);
                }
                catch
                {
                    SetValidationResults(false, nameof(Adress), ["Невозможно получить адрес"]);
                }
            });
        }
    }

    internal sealed class BasketViewModel : ViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }

        public ObservableCollection<BasketCardViewModel> Products { get; }

        private bool _isSingle;

        public bool IsSingle
        {
            get => _isSingle;
            set => SetValue(ref _isSingle, value);
        }

        private readonly User _user;

        public BasketViewModel(User user, Order? order)
        {
            _user = user;

            using AppContext appContext = new();

            List<Order>? ordersInCart;

            if (order != null)
            {
                ordersInCart = [order];
                _isSingle = true;
            }
            else
                ordersInCart = appContext.Orders
                    .Where(o => o.Status == OrderStatus.InCart)
                    .ToList();

            Products = new(
                ordersInCart.Select(o =>
                {
                    var product = appContext.Products.Find(o.ProductId)!;
                    var images = appContext.ProductImages
                        .Where(i => i.ProductId == o.ProductId)
                        .ToList();

                    return new BasketCardViewModel(
                        o, product, images, Navigator, DeleteItem, WatchItem, RemoveInCollection);
                })
            );
        }

        private void DeleteItem(BasketCardViewModel item)
        {
            using AppContext appContext = new();
            appContext.Orders.Remove(item.Order);
            appContext.SaveChanges();
            Products.Remove(item);
        }

        private void WatchItem(BasketCardViewModel item)
        {
            Navigator!.Navigate(new ProductPage(_user, item.Card));
        }

        private void RemoveInCollection(BasketCardViewModel item) => Products.Remove(item);
    }
}