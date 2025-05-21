using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Presentation.ViewModels.AdminPart;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using Mapsui;
using Mapsui.UI.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace course_oop.Presentation.ViewModels.CouriersPart
{
    internal class ShopView ( IFeature feature ) : ViewModel
    {
        IFeature _feature = feature;

        readonly Saller _saller = (Saller?)feature[nameof(Shop.Saller)]!;

        public string Id => $"Магазин №{_feature[nameof(Shop.Id)]}";

        public string Name => $"Название: {_feature[nameof(Shop.Name)]}";

        public string Adress => $"Адрес: {_feature[nameof(Shop.Adress)]}";

        public string OwnersPhone => $"Телефон {_saller.Phone}";

    }

    internal sealed class OrderView ( IFeature feature) : ViewModel
    {
        IFeature _feature = feature;
        readonly Product _product = (Product?)feature[nameof(Order.ProductId)]!;
        readonly User _user = (User?)feature[nameof(Order.UserId)]!;


        public string Id => $"Заказ №{_feature[nameof(Order.Id)]}";


        public string Name => $"Товар: {_product.Name}";


        public string UserFio => $"ФИ заказчика: {_user.FirstName} {_user.Name}";

        public string ProductId => $"Номер товара: {_product.Id}";

        public string ProductName => $"Название товара: {_product.Name}";

    

        public string Adress => $"Адрес доставки {_feature[nameof(Order.DeliveryAddress)]}";

        public string Date => $"Дата заказа {_feature[nameof(Order.CreatedDate)]}";

        public string Status => $"Статус: {GetStatusDisplayName((OrderStatus)_feature[nameof(Order.Status)]!)}";

        public string Price => $"Стоимость: {string.Format("{0:C2}", _product.Price)}";



        private static string GetStatusDisplayName ( OrderStatus status )
        {
            var fieldInfo = status.GetType().GetField(status.ToString());
            var attribute = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false)
                .FirstOrDefault() as DisplayAttribute;

            return attribute?.Name ?? status.ToString();
        }
    }

    //internal sealed class CourierView ( IFeature feature, MapViewModel vm ) : ViewModel
    //{
    //    IFeature _feature = feature;

    //    public string Id => $"Курьер № {_feature[nameof(Core.Entities.Courier.Id)]}";
    //    public string Email => $"Почта курьера: {_feature[nameof(Core.Entities.Courier.Email)]}";
    //    public string Phone => $"Телефон курьера: {_feature[nameof(Core.Entities.Courier.Phone)]}";

    //    public string Fio =>
    //        $"ФИ курьера {_feature[nameof(Core.Entities.Courier.FirstName)]} {_feature[nameof(Core.Entities.Courier.Name)]}";

    //    public double Weight => (double)_feature[nameof(Core.Entities.Courier.Veight)]!;

    //    public double CurrentWeight => (double)_feature[nameof(Core.Entities.Courier.CurrentWeight)]!;

    //    public string WeightStr => $"{_feature[nameof(Core.Entities.Courier.Veight)]} кг";

    //    public string CurrentWeightStr => $"{_feature[nameof(Core.Entities.Courier.CurrentWeight)]} кг";

    //    public string WorkStatus =>
    //        $"Занятость: {(GetOrder((int)_feature[nameof(Core.Entities.Courier.Id)]!) ? "Работает" : "Простаивает")}";

    //    public bool IsAddOrderVisible => vm.Order != null && WorkStatus == "Занятость: Простаивает";

    //    private bool GetOrder ( int id )
    //    {
    //        using AppContext context = new();

    //        return context.Orders.Where(o => o.CourierId == id).Count() != 0;
    //    }
    //}


    internal sealed class EmptyMess
    {
        public string Message => "Выберете пунет марсшрута";
    }

    class MapVM : ValidatedViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }

        private readonly Courier _courier;
        private readonly Shop _shop;
        private readonly Order _order;


        private readonly MapControl _control;

        private bool _dataLoaded;
        private string _loadingMsg;

        Func<IFeature, bool>? _shopFilter;
        Func<IFeature, bool>? _orderFilter;
        Func<IFeature, bool>? _courierFilter;



        public string LoadingMsg
        {
            get => _loadingMsg;
            set { this.SetValue(ref _loadingMsg, value); }
        }

        public bool DataLoaded
        {
            get => _dataLoaded;
            set
            {
                SetValue(ref _dataLoaded, value);
            }
        }

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

        private dynamic _currentView;

        public dynamic? CurrentView
        {
            get => _currentView;
            set
            {
                SetValue(ref _currentView, value);
                _validFields[3] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));

            }
        }

        public ICommand SetCourierStatus => new Command( async () =>
        {
            try
            {
                DataLoaded = false;
                using AppContext context = new();

                var courierTask = context.Couriers.FindAsync(_courier.Id);

                var adressTask = Core.Services.MapServise.GetCoordsByQuery(Adress);

                var courier = await courierTask;
                var adress = await adressTask;

                courier!.Adress = adress.adress;
                courier!.X = adress.x;
                courier!.Y = adress.y;
                courier.StatusMessage = Message;
                
                if(CurrentView is ShopView) courier.SallerMinutes = Minutes;
                else courier.UserMinutes = Minutes;

                context.SaveChanges();
                Adress = string.Empty;
                Message = string.Empty;
                Minutes = 0;
                Array.Fill(_validFields, false);
                OnPropertyChanged(nameof(IsButtonEnabled));
                DataLoaded = true;
            }
            catch (Exception ex)
            {
                SetValidationResults(false, nameof(Adress), [ex.Message]);
            }
        });



        private string _adress;

        [Required(ErrorMessage ="Обязательное поле")]
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

        private string _message;

        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(140, ErrorMessage = "Максимум 140 символов")]

        public string Message
        {
            get => _message;
            set
            {
                SetValue(ref _message, value);
                Validate(Message);
                _validFields[1] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        private int _minutes;


        [Required(ErrorMessage = "Обязательное поле")]
        [Range(0, 75, ErrorMessage = "Максимальное время 75 минут, в случае форс мажора введите макс. время и свяжитесь с администратором")]
        public int Minutes
        {
            get => _minutes;
            set
            {
                SetValue(ref _minutes, value);
                Validate(Minutes);
                _validFields[2] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }


        public async Task UpdteLayers ()
        {
            CourierFilter = f =>
            {
                return (int)f[nameof(Courier.Id)]! == _courier.Id;
            };


            OrderFilter = f => (int)f[nameof(Core.Entities.Order.Id)]! == _order!.Id;

            ShopFilter = f => (int)f[nameof(Core.Entities.Shop.Id)]! == _order!.ShopId;


            var ordersTask = UpdateLayerAsync("Orders", OrderFilter, MapServise.GetOrders);
            var shopsTask = UpdateLayerAsync("Shops", ShopFilter, MapServise.GetShops);
            var couriersTask = UpdateLayerAsync("Couriers", CourierFilter, MapServise.GetCouriers);

            await Task.WhenAll(ordersTask, shopsTask, couriersTask);

            _control.Map.Refresh();

            DataLoaded = true;
        }

        private async Task UpdateLayerAsync (
            string layerName,
            Func<IFeature, bool>? filter,
            Func<Map, Func<IFeature, bool>?, Task<Map>> getLayerFunc )
        {
            _control.Map = await MapServise.RemoveLayer(_control.Map, layerName);
            _control.Map = await getLayerFunc(_control.Map, filter);
        }

        public Core.Services.MapServise MapServise { get; } = new(couriers: true, shops: true, clients: true);

        public MapVM( ref MapControl control, Courier courier)
        {
            _courier = courier;
            _control = control;
            _dataLoaded = false;
            _loadingMsg = "Загрузка";
            _control = control;

            _validFields = new bool[4];

            _adress = string.Empty;
            _message = string.Empty;

            _currentView = new EmptyMess();

            using AppContext context = new();

            _order = context.Orders.Where(o => o.CourierId == _courier.Id).First();

            _shop = context.Shops.Find(_order.ShopId)!;
            _control.Info += Map_Info;
        }

        private void Map_Info ( object? sender, MapInfoEventArgs e )
        {
            if (e.MapInfo != null && e.MapInfo.Feature != null)
            {
                if ((string)e.MapInfo.Feature["Type"]! == "Shop")
                {
                    this.CurrentView = new ShopView(e.MapInfo.Feature);
                }

                if ((string)e.MapInfo.Feature["Type"]! == "Order")
                {
                    this.CurrentView = new OrderView(e.MapInfo.Feature);
                }

                //if ((string)e.MapInfo.Feature["Type"]! == "Courier")
                //{
                //    this.CurrentView = new CourierView(e.MapInfo.Feature);
                //}
            }
        }
    }
}
