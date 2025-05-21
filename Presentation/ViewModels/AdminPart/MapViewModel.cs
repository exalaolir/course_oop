using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.Views.Admin;
using course_oop.Shared.Const;
using HarfBuzzSharp;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.UI.Wpf;

namespace course_oop.Presentation.ViewModels.AdminPart
{
    internal class ShopView(IFeature feature) : ViewModel
    {
        IFeature _feature = feature;

        readonly Saller _saller = (Saller?)feature[nameof(Shop.Saller)]!;

        public string Id => $"Магазин №{_feature[nameof(Shop.Id)]}";

        public string Name => $"Название: {_feature[nameof(Shop.Name)]}";

        public string Description => $"Описание: {_feature[nameof(Shop.Description)]}";

        public string Adress => $"Адрес: {_feature[nameof(Shop.Adress)]}";

        public string OwnersNumber => $"Номер владельца {_saller.SalersId}";

        public string OwnersPhone => $"Телефон {_saller.Phone}";

        public string OwnersEmail => $"Email {_saller.Email}";
    }

    internal sealed class OrderView(IFeature feature, MapViewModel vm) : ViewModel
    {
        IFeature _feature = feature;
        readonly Product _product = (Product?)feature[nameof(Order.ProductId)]!;
        readonly User _user = (User?)feature[nameof(Order.UserId)]!;

       
        public string Id => $"Заказ №{_feature[nameof(Order.Id)]}";

        public ICommand SetCouriers => new Command(() =>
        {
            vm.GetOrder(vm.AllOrders.First(o => o.Order.Id == (int)_feature[nameof(Order.Id)]!));
        });

        public string Name => $"Товар: {_product.Name}";

        public string UserNumber => $"Id заказчика: {_user.Id}";

        public string UserFio => $"ФИ заказчика: {_user.FirstName} {_user.Name}";

        public string ProductId => $"Номер товара: {_product.Id}";

        public string ProductName => $"Название товара: {_product.Name}";

        public string Weight => $"Вес товара: {_product.Weight} кг";

        public string Adress => $"Адрес доставки {_feature[nameof(Order.DeliveryAddress)]}";

        public string Date => $"Дата заказа {_feature[nameof(Order.CreatedDate)]}";

        public string Status => $"Статус: {GetStatusDisplayName((OrderStatus)_feature[nameof(Order.Status)]!)}";

        public bool BtnVisible => Status == "Статус: Готов к выдаче курьеру" && vm.Order == null;

        public string Price => $"Стоимость: {string.Format("{0:C2}", _product.Price)}";

        

        private static string GetStatusDisplayName(OrderStatus status)
        {
            var fieldInfo = status.GetType().GetField(status.ToString());
            var attribute = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            return attribute?.Name ?? status.ToString();
        }
    }

    internal sealed class CourierView: ViewModel
    {
        IFeature _feature;
        MapViewModel vm;

        public string Id => $"Курьер № {_feature[nameof(Core.Entities.Courier.Id)]}";
        public string Email => $"Почта курьера: {_feature[nameof(Core.Entities.Courier.Email)]}";
        public string Phone => $"Телефон курьера: {_feature[nameof(Core.Entities.Courier.Phone)]}";

        public string Fio =>
            $"ФИ курьера {_feature[nameof(Core.Entities.Courier.FirstName)]} {_feature[nameof(Core.Entities.Courier.Name)]}";

        public double Weight => (double)_feature[nameof(Core.Entities.Courier.Veight)]!;

        public double CurrentWeight => (double)_feature[nameof(Core.Entities.Courier.CurrentWeight)]!;

        public string WeightStr => $"{_feature[nameof(Core.Entities.Courier.Veight)]} кг";

        public string CurrentWeightStr => $"{_feature[nameof(Core.Entities.Courier.CurrentWeight)]} кг";

        public string WorkStatus =>
            $"Занятость: {(GetOrder((int)_feature[nameof(Core.Entities.Courier.Id)]!) ? "Работает" : "Простаивает")}";

        public string Adress =>
            $"Адрес: {(((string?)_feature[nameof(Core.Entities.Courier.Adress)]! is null) ? "В точке старта" : (string)_feature[nameof(Core.Entities.Courier.Adress)]!)}";

        public string Message =>
            $"{(((string?)_feature[nameof(Core.Entities.Courier.StatusMessage)]! is null) ? "Сообщение:" : "Сообщение: " + (string)_feature[nameof(Core.Entities.Courier.StatusMessage)]!)}";

        public string? Time { get; }


        public CourierView ( IFeature feature, MapViewModel vm )
        {
            _feature = feature;
            this.vm = vm;

            Time = "Прибудет через: Неизвестно";

            if(vm?.Order?.Order.Status == OrderStatus.WaitCourier)
            {
                Time = $"Прибудет через: {((int?)_feature[nameof(Courier.SallerMinutes)] ?? 120)} минут(у)";
            }
            else if(vm?.Order?.Order.Status == OrderStatus.InDelivery)
            {
                int data = (int?)_feature[nameof(Courier.UserMinutes)] ?? 120;
                Time = $"Прибудет через: {data} минут(у)";
            }
        }

        public bool IsAddOrderVisible => vm.Order != null && WorkStatus == "Занятость: Простаивает";

        public ICommand AddCommand => new Command(() =>
        {
            using AppContext context = new();

            var courier = context.Couriers.Find(_feature[nameof(Core.Entities.Courier.Id)]!);
            var order = context.Orders.Find(vm.Order!.Order.Id);
            var product = context.Products.Find(order!.ProductId);
            courier!.IsWork = true;

            courier.CurrentWeight = courier.CurrentWeight == null ? (double)product!.Weight : courier.CurrentWeight + courier.CurrentWeight;

            order.CourierId = courier.Id;
            order.Status = OrderStatus.WaitCourier;
            context.SaveChanges();

            vm.Navigator!.Navigate(new MapPage(order));
        });

        private bool GetOrder(int id)
        {
            using AppContext context = new();

            return context.Orders.Where(o => o.CourierId == id).Count() != 0;
        }
    }

    internal sealed class OrderCardViewModel(Order order, Product product)
    {
        public Order Order => order;
        public Product Product => product;

        public string OrderNumber => $"№{Order.Id}: ";

        public string ProductName => Product.Name;
    }

    internal class MapViewModel : ViewModel, INavigated
    {
        private bool _dataLoaded;
        private string _loadingMsg;
        private MapControl _control;
        private bool _curierLayer = true;
        private bool _shopLayer = true;
        private bool _orderLayer = true;
        private dynamic? _currentView;
        private OrderCardViewModel? _order;
        Func<IFeature, bool>? _shopFilter;
        Func<IFeature, bool>? _orderFilter;
        Func<IFeature, bool>? _courierFilter;

        public INavigation? Navigator { get; set; }

        public bool DataLoaded
        {
            get => _dataLoaded;
            set => this.SetValue(ref _dataLoaded, value);
        }

        public ObservableCollection<OrderCardViewModel> Orders { get; set; }

        public ObservableCollection<OrderCardViewModel> AllOrders { get; set; }

        public string LoadingMsg
        {
            get => _loadingMsg;
            set { this.SetValue(ref _loadingMsg, value); }
        }

        public bool ShopLayer
        {
            get => _shopLayer;
            set => SetValue(ref _shopLayer, value);
        }

        public bool CurierLayer
        {
            get => _curierLayer;
            set => SetValue(ref _curierLayer, value);
        }

        public bool OrderLayer
        {
            get => _orderLayer;
            set => SetValue(ref _orderLayer, value);
        }

        private bool _isWatedOrders = true;

        public bool IsWatedOrders
        {
            get => _isWatedOrders;
            set
            {
                SetValue(ref _isWatedOrders, value);
            }
        }

        private bool _isInDeliveryOrders = true;

        public bool IsInDeliveryOrders
        {
            get => _isInDeliveryOrders;
            set
            {
                SetValue(ref _isInDeliveryOrders, value);
            }
        }

        public ICommand FilterListByStatus { get; }

        public dynamic? CurrentView
        {
            get => _currentView;
            set => SetValue(ref _currentView, value);
        }

        public ICommand ApplyLayersCommand { get; }

        public OrderCardViewModel? Order
        {
            get => _order;
            set
            {
                SetValue(ref _order, value);
                
                if (value != null)
                {

                    Task.Run(async () =>
                    {
                        await UpdteLayers();
                        var layer = _control.Map.Layers
                            .FindLayer("Orders")
                            .First() as MemoryLayer;
                        this.CurrentView = new OrderView(layer!.Features.First(), this);
                    });
                }
            }
        }

        private string _searchText = string.Empty;

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetValue(ref _searchText, value);
            }
        }



        public Func<OrderCardViewModel, bool>? SearchFilter { get; set; }

        public ICommand GetFilterCommand => new Command<string>(name =>
        {
            switch(name)
            {
                case "1":
                    SearchFilter = o => Regex.IsMatch(o.OrderNumber, $"^{Regex.Escape(SearchText)}");
                    break;
                case "2":
                    SearchFilter = o => Regex.IsMatch(o.ProductName, $"^{Regex.Escape(SearchText)}");
                    break;
                case "3":
                    SearchFilter = o => Regex.IsMatch(o.Order.DeliveryAddress!, $"^{Regex.Escape(SearchText)}");
                    break;
            }
        });

        public ICommand SearchCommand { get; }

        public Func<IFeature, bool>? ShopFilter
        {
            get => _shopFilter;
            set => SetValue(ref _shopFilter, value);
        }

        public Func<IFeature, bool>? OrderFilter
        {
            get => _orderFilter;
            set => SetValue(ref _orderFilter, value);
        }

        public Func<IFeature, bool>? CourierFilter
        {
            get => _courierFilter;
            set => SetValue(ref _courierFilter, value);
        }

        private async Task UpdteLayers()
        {
            CurierLayer = true;
            OrderLayer = true;
            ShopLayer = true;

            CourierFilter = f =>
            {
                if (_order!.Order.CourierId == null)
                {
                    return (bool)f[nameof(Courier.IsWork)]! == false &&
                    (double)f[nameof(Courier.Veight)]! - (double)f[nameof(Courier.CurrentWeight)]! <= (double)Order!.Product.Weight;
                }
                else
                {
                    using AppContext appContext = new AppContext();
                    var newCourier = appContext.Couriers.Find(f[nameof(Courier.Id)]);

                    return _order!.Order.CourierId == newCourier!.Id;
                }
            };


            OrderFilter = f => (int)f[nameof(Core.Entities.Order.Id)]! == _order!.Order.Id;

            ShopFilter = f => (int)f[nameof(Core.Entities.Shop.Id)]! == _order!.Order.ShopId;


            var ordersTask = UpdateLayerAsync("Orders", OrderFilter, MapServise.GetOrders);
            var shopsTask = UpdateLayerAsync("Shops", ShopFilter, MapServise.GetShops);
            var couriersTask = UpdateLayerAsync("Couriers", CourierFilter, MapServise.GetCouriers);

            await Task.WhenAll(ordersTask, shopsTask, couriersTask);

            _control.Map.Refresh();
        }

        private async Task UpdateLayerAsync(
            string layerName,
            Func<IFeature, bool>? filter,
            Func<Map, Func<IFeature, bool>?, Task<Map>> getLayerFunc)
        {
            _control.Map = await MapServise.RemoveLayer(_control.Map, layerName);
            _control.Map = await getLayerFunc(_control.Map, filter);
        }


        public Core.Services.MapServise MapServise { get; } = new(couriers: true, shops: true, clients: true);

        public ICommand ResetAll => new Command(() => Navigator!.Navigate(new MapPage()));

        public MapViewModel(ref MapControl control, Order? param)
        {
            _loadingMsg = "Загрузка";
            _control = control;
            _control.Info += Map_Info;

            Orders = [];

            using AppContext appContext = new();

            var collection = appContext.Orders.Where(o =>
                o.Status != OrderStatus.InCart && o.Status != OrderStatus.Delivered &&
                o.Status != OrderStatus.Processing).ToList();

            collection.ForEach(o =>
            {
                var product = appContext.Products.Find(o.ProductId);
                Orders.Add(new OrderCardViewModel(o, product!));
            });

            AllOrders = new ObservableCollection<OrderCardViewModel>(Orders);

            ApplyLayersCommand = new Command(async () =>
            {
                try
                {
                    LoadingMsg = "Загрузка";
                    DataLoaded = false;
                    _control.Map = await MapServise.RemoveLayer(_control.Map, "Couriers");
                    _control.Map = await MapServise.RemoveLayer(_control.Map, "Orders");
                    _control.Map = await MapServise.RemoveLayer(_control.Map, "Shops");

                    if (CurierLayer)
                        await UpdateLayerAsync("Couriers", CourierFilter, MapServise.GetCouriers);


                    if (ShopLayer)
                        await UpdateLayerAsync("Shops", ShopFilter, MapServise.GetShops);

                    if (OrderLayer)

                        await UpdateLayerAsync("Orders", OrderFilter, MapServise.GetOrders);

                    DataLoaded = true;
                }
                catch (Exception ex)
                {
                    LoadingMsg = ex.Message;
                }
            });

            FilterListByStatus = new Command(FilterListByStatusMethod);

            SearchFilter = o => Regex.IsMatch(o.OrderNumber, $"^{Regex.Escape(SearchText)}");

            Order = param == null ? null: new OrderCardViewModel(param, appContext.Products.Find(param.ProductId)!);

            SearchCommand = new Command(() =>
            {
                FilterListByStatusMethod();
                Orders = new(Orders.Where(SearchFilter!));
                OnPropertyChanged(nameof(Orders));
            });
        }

        private void FilterListByStatusMethod ()
        {
            var container = Order;
            Func<OrderCardViewModel, bool> filter = o =>
            {
                bool filter1 = IsWatedOrders ? o.Order.Status == OrderStatus.ReadyForCourier : false;

                bool filter2 = IsInDeliveryOrders ? o.Order.Status == OrderStatus.InDelivery || o.Order.Status == OrderStatus.WaitCourier : false;

                return filter1 || filter2;
            };

            Orders = new(AllOrders.Where(filter));
            OnPropertyChanged(nameof(Orders));
            Order = container;
        }

        internal void GetOrder ( OrderCardViewModel order ) => Order = order;

        private void Map_Info(object? sender, MapInfoEventArgs e)
        {
            if (e.MapInfo != null && e.MapInfo.Feature != null)
            {
                if ((string)e.MapInfo.Feature["Type"]! == "Shop")
                {
                    this.CurrentView = new ShopView(e.MapInfo.Feature);
                }

                if ((string)e.MapInfo.Feature["Type"]! == "Order")
                {
                    this.CurrentView = new OrderView(e.MapInfo.Feature, this);
                }

                if ((string)e.MapInfo.Feature["Type"]! == "Courier")
                {
                    this.CurrentView = new CourierView(e.MapInfo.Feature, this);
                }
            }
        }
    }
}