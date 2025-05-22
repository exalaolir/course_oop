using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Infrastructure.Data.Repositories;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.ViewModels.Components;
using course_oop.Presentation.Views.Saller;
using Microsoft.EntityFrameworkCore;

namespace course_oop.Presentation.ViewModels.SallersPart
{
    internal class GoodsViewModel : ValidatedViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }

        public bool IsEditable
        {
            get => _isEditable;
            set => SetValue(ref _isEditable, value);
        }

        private readonly Saller _saller;

        private bool _isEditable;


        private Shop? _shop;

        public ICommand? CreatePage { get; }

        public ICommand? DeleteProduct { get; }

        public ICommand? CreateNewPage { get; }
        public ObservableCollection<Product>? Products { get; set; }

        public GoodsViewModel(Saller saller)
        {
            _saller = saller;

            using Repo repo = new();

            _shop = repo.GetShop(_saller.Id).Cast<Shop>().FirstOrDefault();

            using AppContext appContext = new AppContext(); 

           

            IsEditable = _shop != null;

            if (IsEditable)
            {
                Products = new(repo.GetProducts(_shop!.Id).Cast<Product>());

                CreatePage = new Command<Product?>(p => Navigator!.Navigate(new ProductPage(p, _saller, _shop)));

                CreateNewPage = new Command(() => Navigator!.Navigate(new ProductPage(null, _saller, _shop)));

                DeleteProduct = new Command<Product>(p =>
                {
                  
                    Products.Remove(p);
                    using AppContext context = new AppContext();



                    List<int?> nums = [];

                    context.Orders.Where(o => o.Product!.Id == p.Id).ToList().ForEach(i => nums.Add(i.CourierId));


                    context.Orders.Where(o => o.Product!.Id == p.Id)
                    .ExecuteUpdate(o => o
                        .SetProperty(x => x.Status, OrderStatus.Rejected)
                        .SetProperty(x => x.CourierId, (int?)null)
                      );

                    double weight = (double)context.Products.Where(p => p.Id == p.Id).Sum(p => p.Weight);

                    context.Couriers.Where(p => nums.Contains(p.Id))
                        .ExecuteUpdate(c => c.
                        SetProperty(c => c.SallerMinutes, (int?)null)
                        .SetProperty(c => c.UserMinutes, (int?)null)
                        .SetProperty(c => c.CurrentWeight, c => c.CurrentWeight - weight)
                        .SetProperty(c => c.IsWork, false));

                    var productsInCategory = context.Products
                        .Where(z => z.Id == p.Id)
                        .Include(z => z.Reviews) // Важно: подгружаем отзывы
                        .ToList();

                    // Собираем ВСЕ отзывы этих продуктов в один список
                    var allReviewsToDelete = productsInCategory
                        .SelectMany(p => p.Reviews) // "Разворачиваем" коллекции отзывов
                        .ToList();

                    // Удаляем все отзывы разом
                    context.Rewiews.RemoveRange(allReviewsToDelete);

               
                    context.Products.Remove(context.Products.Find(p.Id)!);

                    context.SaveChanges();
                });
            }
        }
    }
}