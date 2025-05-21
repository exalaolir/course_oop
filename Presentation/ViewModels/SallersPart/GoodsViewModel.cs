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

            IsEditable = _shop != null;

            if (IsEditable)
            {
                Products = new(repo.GetProducts(_shop!.Id).Cast<Product>());

                CreatePage = new Command<Product?>(p => Navigator!.Navigate(new ProductPage(p, _saller, _shop)));

                CreateNewPage = new Command(() => Navigator!.Navigate(new ProductPage(null, _saller, _shop)));

                DeleteProduct = new Command<Product>(p =>
                {
                    using Repo repo = new();
                    Products.Remove(p);
                    repo.RemoveProduct(p.Id);
                });
            }
        }
    }
}