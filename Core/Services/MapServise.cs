using System.Globalization;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using Windows.Data.Json;
using course_oop.Core.Entities;
using course_oop.Infrastructure.Data.Repositories;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Serilog;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace course_oop.Core.Services
{
    internal class MapServise(bool couriers = false, bool clients = false, bool shops = false)
    {
        private readonly bool _couriers = couriers;
        private readonly bool _clients = clients;
        private readonly bool _shops = shops;
        private readonly object _locker = new object();

        public async Task<Map> LoadMap()
        {
            if (!await CheckInternetConnectionAsync())
            {
                throw new HttpRequestException("Нет подключения к интернету");
            }

            return await Task.Run(() =>
            {
                var map = new Map();

                map.Layers.Add(OpenStreetMap.CreateTileLayer());

                if (_shops) map.Layers.Add(CreateShopsLayer(null));

                if (_couriers) map.Layers.Add(CreateCouriersLayer(null));

                if (_clients) map.Layers.Add(CreateOrdersLayer(null));

                map.Navigator.OverrideZoomBounds = new(20135.758475, 2.38865713348);

                var center = SphericalMercator.FromLonLat(27.55974390187122, 53.89157870614333);

                map.Home = n => map.Navigator.CenterOnAndZoomTo(new(center.x, center.y), resolution: n.Resolutions[12]);

                return map;
            });
        }

        public async Task<Map> GetCouriers(Map priviosMap, Func<IFeature, bool>? filter = null)
        {
            if (!await CheckInternetConnectionAsync())
            {
                throw new HttpRequestException("Нет подключения к интернету");
            }

            return await Task.Run(() =>
            {
                var map = priviosMap;

                var newLayer = CreateCouriersLayer(filter);

                lock (_locker)
                {
                    if (_couriers) map.Layers.Add(newLayer);

                    var center = SphericalMercator.FromLonLat(27.55974390187122, 53.89157870614333);

                    map.Home = n =>
                        map.Navigator.CenterOnAndZoomTo(new(center.x, center.y), resolution: n.Resolutions[12]);

                    map.Refresh();

                    return map;
                }
            });
        }

        public async Task<Map> GetShops(Map priviosMap, Func<IFeature, bool>? filter = null)
        {
            if (!await CheckInternetConnectionAsync())
            {
                throw new HttpRequestException("Нет подключения к интернету");
            }

            return await Task.Run(() =>
            {
                var map = priviosMap;

                var newLayer = CreateShopsLayer(filter);

                lock (_locker)
                {
                    if (_shops) map.Layers.Add(newLayer);

                    var center = SphericalMercator.FromLonLat(27.55974390187122, 53.89157870614333);

                    map.Home = n =>
                        map.Navigator.CenterOnAndZoomTo(new(center.x, center.y), resolution: n.Resolutions[12]);

                    map.Refresh();

                    return map;
                }
            });
        }

        public async Task<Map> GetOrders(Map priviosMap, Func<IFeature, bool>? filter = null)
        {
            if (!await CheckInternetConnectionAsync())
            {
                throw new HttpRequestException("Нет подключения к интернету");
            }

            return await Task.Run(() =>
            {
                var map = priviosMap;

                var newLayer = CreateOrdersLayer(filter);

                lock (_locker)
                {
                    if (_clients) map.Layers.Add(newLayer);

                    var center = SphericalMercator.FromLonLat(27.55974390187122, 53.89157870614333);

                    map.Home = n =>
                        map.Navigator.CenterOnAndZoomTo(new(center.x, center.y), resolution: n.Resolutions[12]);

                    map.Refresh();

                    return map;
                }
            });
        }


        public async Task<Map> RemoveLayer(Map priviosMap, string name)
        {
            if (!await CheckInternetConnectionAsync())
            {
                throw new HttpRequestException("Нет подключения к интернету");
            }

            return await Task.Run(() =>
            {
                lock (_locker)
                {
                    var map = priviosMap;

                    var layer = map.Layers.FindLayer(name).FirstOrDefault();
                    if (layer != null) map.Layers.Remove(layer);

                    var center = SphericalMercator.FromLonLat(27.55974390187122, 53.89157870614333);

                    map.Home = n =>
                        map.Navigator.CenterOnAndZoomTo(new(center.x, center.y), resolution: n.Resolutions[12]);

                    map.Refresh();

                    return map;
                }
            });
        }

        private MemoryLayer CreateShopsLayer(Func<IFeature, bool>? filter)
        {
            return new MemoryLayer
            {
                Name = "Shops",
                IsMapInfoLayer = true,
                Style = SymbolStyles.CreatePinStyle(symbolScale: 1.1, pinColor: Color.FromArgb(255, 108, 203, 96)),
                Features = filter == null
                    ? new MemoryProvider(GetShops()).Features
                    : new MemoryProvider(GetShops()).Features.Where(filter).ToList(),
            };
        }

        private MemoryLayer CreateCouriersLayer(Func<IFeature, bool>? filter = null)
        {
            return new MemoryLayer
            {
                Name = "Couriers",
                IsMapInfoLayer = true,
                Style = SymbolStyles.CreatePinStyle(symbolScale: 1, pinColor: Color.FromArgb(255, 255, 153, 164)),
                Features = filter == null
                    ? new MemoryProvider(GetCouriers(filter)).Features
                    : new MemoryProvider(GetCouriers(filter)).Features.Where(filter).ToList(),
            };
        }

        private MemoryLayer CreateOrdersLayer(Func<IFeature, bool>? filter)
        {
            return new MemoryLayer
            {
                Name = "Orders",
                IsMapInfoLayer = true,
                Style = SymbolStyles.CreatePinStyle(symbolScale: 0.9, pinColor: Color.FromArgb(255, 252, 255, 0)),
                Features = filter == null
                    ? new MemoryProvider(GetOrders()).Features
                    : new MemoryProvider(GetOrders()).Features.Where(filter).ToList()
            };
        }

        private IEnumerable<IFeature> GetCouriers(Func<IFeature, bool>? filter = null)
        {
            Repo repository = new();

            var data = repository.GetNotBannedCouriers().Cast<Courier>().Select(e =>
            {
                var point = e.X == null ?
                    SphericalMercator.FromLonLat( 27.55974390187122, 53.89157870614333).ToMPoint()
                    :new MPoint(x: (double)e.X!, y: (double)e.Y!);

                var feature = new PointFeature(
                   point);

                feature[nameof(Courier.Id)] = e.Id;
                feature["Type"] = "Courier";
                feature[nameof(Courier.Email)] = e.Email ?? string.Empty;
                feature[nameof(Courier.Adress)] = e.Adress;
                feature[nameof(Courier.StatusMessage)] = e.StatusMessage;
                feature[nameof(Courier.SallerMinutes)] = e.SallerMinutes;
                feature[nameof(Courier.UserMinutes)] = e.UserMinutes;
                feature[nameof(Courier.CurrentWeight)] = e.CurrentWeight ?? 0;
                feature[nameof(Courier.Phone)] = e.Phone ?? string.Empty;
                feature[nameof(Courier.Name)] = e.Name ?? string.Empty;
                feature[nameof(Courier.FirstName)] = e.FirstName ?? string.Empty;
                feature[nameof(Courier.Banned)] = e.Banned;
                feature[nameof(Courier.Transport)] = e.Transport?.ToString() ?? "Не указан";
                feature[nameof(Courier.Veight)] = e.Veight ?? 0;
                feature[nameof(Courier.IsWork)] = e.IsWork ?? false;

                return feature;
            }).ToList();

            repository.Dispose();
            return data;
        }

        private IEnumerable<IFeature> GetShops()
        {
            Repo repository = new();
            AppContext appContext = new AppContext();
            var data = repository.GetShops().Cast<Shop>().Select(e =>
            {
                var feature = new PointFeature(
                    new MPoint(x: (double)e.X!, y: (double)e.Y!));
                var saller = appContext.Sallers.Where(s => s.Id == e.SallerId).ToList().First();
                feature[nameof(Shop.Id)] = e.Id;
                feature["Type"] = "Shop";
                feature[nameof(Shop.Name)] = e.Name ?? string.Empty;
                feature[nameof(Shop.Description)] = e.Description ?? string.Empty;
                feature[nameof(Shop.Adress)] = e.Adress ?? string.Empty;
                feature[nameof(Shop.Saller)] = saller;
                return feature;
            }).ToList();

            repository.Dispose();
            appContext.Dispose();
            return data;
        }


        private IEnumerable<IFeature> GetOrders()
        {
            AppContext appContext = new AppContext();
            var data = appContext.Orders.ToList()
                .Where(o => o.Status == OrderStatus.InDelivery || o.Status == OrderStatus.ReadyForCourier || o.Status == OrderStatus.WaitCourier).Select(e =>
                {
                    var feature = new PointFeature(
                        new MPoint(x: (double)e.X!, y: (double)e.Y!));

                    feature[nameof(Order.Id)] = e.Id;
                    feature["Type"] = "Order";
                    feature[nameof(Order.Status)] = e.Status;
                    feature[nameof(Order.CreatedDate)] = e.CreatedDate;
                    feature[nameof(Order.DeliveryAddress)] = e.DeliveryAddress;
                    feature[nameof(Order.ShopId)] = appContext.Shops.Where(s => s.Id == e.ShopId).ToList().First();
                    feature[nameof(Order.ProductId)] =
                        appContext.Products.Where(s => s.Id == e.ProductId).ToList().First();
                    feature[nameof(Order.UserId)] = appContext.Users.Where(s => s.Id == e.UserId).ToList().First();
                    feature[nameof(Order.CourierId)] =
                        appContext.Couriers.Where(s => s.Id == e.CourierId).ToList().FirstOrDefault();
                    return feature;
                }).ToList();

            appContext.Dispose();
            return data;
        }


        private static async Task<bool> CheckInternetConnectionAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    var response = await client.GetAsync("http://www.google.com");
                    return response.IsSuccessStatusCode;
                }
            }
            catch
            {
                return false;
            }
        }

        public static async Task<(double x, double y, string adress)> GetCoordsByQuery(string query)
        {
            if (!await CheckInternetConnectionAsync())
            {
                throw new HttpRequestException("Нет подключения к интернету");
            }

            string appName = "course_project_0.1";
            var requestUrl = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(query)}&format=json";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", appName);
                client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU");

                var response = await client.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    using var jsonResults = JsonDocument.Parse(json);

                    foreach (var root in jsonResults.RootElement.EnumerateArray())
                    {
                        if (root.TryGetProperty("display_name", out var adress))
                        {
                            var adressStr = adress.GetString();
                            if (adressStr != null)
                            {
                                var lat = double.Parse(root.GetProperty("lat").GetString()!, NumberStyles.Any,
                                    CultureInfo.InvariantCulture);
                                var lon = double.Parse(root.GetProperty("lon").GetString()!, NumberStyles.Any,
                                    CultureInfo.InvariantCulture);

                                var points = SphericalMercator.FromLonLat(lon, lat);
                                return (points.x, points.y, adressStr);
                            }
                            else throw new Exception("Адрес не найден");
                        }
                        else throw new Exception("Адрес не найден");
                    }

                    throw new Exception("Адрес не найден");
                }
                else throw new Exception("Неверные данные");
            }
        }
    }
}