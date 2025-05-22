using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.Views.User;
using Panuon.WPF;

namespace course_oop.Presentation.ViewModels.UsersPart
{
    internal sealed class CardVM : ViewModel
    {
        private readonly Order _order;
        private readonly Product _product;
        private readonly Courier? _courier;

        public OrderStatus Status => _order.Status;

        public string Name => $"{_order.Name}";

        public string Price => $"Цена: {string.Format("{0:C2}", _order.Price)}";

        public string Time => $"Прибудет через: {_courier?.UserMinutes?.ToString() ?? "неизвестно"} минут";

        private bool _delVisible;

        public bool DelVisible
        {
            get => _delVisible;
            set
            {
                SetValue(ref _delVisible, value);
            }
        }

        private bool _confirmVisible;

        public bool ConfirmVisible
        {
            get => _confirmVisible;
            set
            {
                SetValue(ref _confirmVisible, value);
            }
        }

        public ICommand DelCommand { get; }

        public ICommand ConfCommand { get; }

        public CardVM(Order order , HomePageViewModel vm)
        {
            _order = order;
            using AppContext appContext = new AppContext();
            _product = appContext.Products.Find(_order.ProductId)!;
            _courier = appContext.Couriers.Find(_order.CourierId);

            if(Status == OrderStatus.InCart || Status == OrderStatus.Processing ) 
                DelVisible = true;

            if (_courier != null
                && _courier?.X == _order.X
                && _courier?.Y == _order.Y)
                ConfirmVisible = true;


            DelCommand = new Command(() =>
            {
                using AppContext context = new();
                var order = context.Orders.Find(_order.Id);

                if (Status == OrderStatus.InCart)
                    context.Orders.Remove(order!);
                else order!.Status = OrderStatus.Rejected;

                context.SaveChanges();
                vm.Navigator!.Navigate(new HomePage(vm._user));
            });

            ConfCommand = new Command(() =>
            {
                using AppContext context = new();
                var order = context.Orders.Find(_order.Id);
                var courier = context.Couriers.Find(order!.CourierId);

                order!.Status = OrderStatus.Delivered;
                order.CourierId = null;
                courier!.IsWork = false;
                courier.CurrentWeight -= (double)_product.Weight;


                context.SaveChanges();
                vm.Navigator!.Navigate(new HomePage(vm._user));
            });
        }
    }


    internal sealed class HomePageViewModel : ViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }

        public readonly User _user;

        public ObservableCollection<CardVM> Orders { get; set; }

        public HomePageViewModel(User user )
        {
            _user = user;

            using AppContext context = new ();
            Orders = [];

            var data = context.Orders.Where(o => o.UserId == _user.Id).ToList();

            data.ForEach(d => Orders.Add(new CardVM(d, this)));
        }
    }
}