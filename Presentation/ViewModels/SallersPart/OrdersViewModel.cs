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
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.Views.Saller;
using Microsoft.EntityFrameworkCore;

namespace course_oop.Presentation.ViewModels.SallersPart
{
    public sealed class Status(string name, OrderStatus status)
    {
        public string Name => name;
        public OrderStatus OrderStatus => status;
    }

    public sealed class OrderCardViewModel : ValidatedViewModel
    {
        private Order _order;
        private Status? _newStatus;
        private readonly Product _product;
        private readonly Action _rerender;
        private Courier? _courier;
        private readonly Shop _shop;

        private bool _btnVisible;

        public bool BtnVisible
        {
            get => _btnVisible;
            set
            {
                SetValue(ref _btnVisible, value);
            }
        }

        private bool _isListVisible;

        public bool IsListVisible
        {
            get => _isListVisible;
            set
            {
                SetValue(ref _isListVisible, value);
            }
        }



        private static ObservableCollection<Status> _statuses =
        [
            new Status(GetStatusDisplayName(OrderStatus.Processing), OrderStatus.Processing),
            new Status(GetStatusDisplayName(OrderStatus.Rejected), OrderStatus.Rejected),
            new Status(GetStatusDisplayName(OrderStatus.ReadyForCourier), OrderStatus.ReadyForCourier),

        ];


        public ICommand GiveToCourier { get; }

        public OrderCardViewModel(Order order, Action rerender)
        {
            _order = order;
            _validFields = new bool[1];
            _rerender = rerender;
            _isListVisible = true;


            using AppContext context = new();
            _product = context.Products.Find(_order.ProductId)!;
            _courier = context.Couriers.Find(_order?.CourierId);
            _shop = context.Shops.Find(_order!.ShopId)!;


            if(_order.Status == OrderStatus.WaitCourier)
            {
                Time = $"Курьер прибудет через {_courier!.SallerMinutes} минут(ы)";

                if (_courier.X == _shop.X && _courier.Y == _shop.Y)
                    BtnVisible = true;
            }

            if (_order != null && _order.Status != OrderStatus.Processing)
                IsListVisible = false;

            ChangeStatusCommand = new Command<OrderCardViewModel>(item =>
            {
                using AppContext context = new();
                context.Orders.Find(item.Id)!.Status = NewStatus!.OrderStatus;
                context.SaveChanges();
                _validFields[0] = false;
                OnPropertyChanged(nameof(IsButtonEnabled));
                _rerender.Invoke();
            });

            GiveToCourier = new Command(() =>
            {
                using AppContext context = new();
                var order = context.Orders.Find(_order.Id)!;

                order.Status = OrderStatus.InDelivery;

                var courier = context.Couriers.Find(_order?.CourierId);


                courier!.SallerMinutes = null;
                context.SaveChanges();
                BtnVisible = false;
                Time = string.Empty;
                OnPropertyChanged(nameof(Time));
                rerender.Invoke();
            });
        }


        public int Id => _order.Id;
        public string Name => _order.Name;
        public Order Order => _order;
        public DateTime CreatedDate => _order.CreatedDate;
        public OrderStatus Status => _order.Status;
        public string StatusDisplayName => GetStatusDisplayName(_order.Status);
        public string? DeliveryAddress => _order.DeliveryAddress;

        public ICommand ChangeStatusCommand { get; }

        public string Time { get; private set; }

        public Status? NewStatus
        {
            get => _newStatus;
            set
            {
                SetValue(ref _newStatus, value);
                _validFields[0] = true;
                if(value != null && value.OrderStatus != OrderStatus.Processing)
                    IsListVisible = false;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public ObservableCollection<Status> Statuses => _statuses;

        private static string GetStatusDisplayName(OrderStatus status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());
            var attribute = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            return attribute?.Name ?? status.ToString();
        }
    }


    internal sealed class OrdersViewModel : ViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }

        private readonly Saller _saller;

        public ObservableCollection<OrderCardViewModel> Orders { get; }

        public OrdersViewModel(Saller saller)
        {
            _saller = saller;

            using AppContext context = new();
            Orders = [];

            var data = context.Orders.Where(o => o.Status != OrderStatus.InCart ).ToList();
            data.ForEach(o => Orders.Add(new OrderCardViewModel(o, Rerender)));
        }

        private void Rerender() => Navigator!.Navigate(new Orders(_saller));
    }
}