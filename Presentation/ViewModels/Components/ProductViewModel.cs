using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Infrastructure.Data.Repositories;
using course_oop.Migrations;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.Views.Saller;
using Microsoft.Win32;

namespace course_oop.Presentation.ViewModels.Components
{
    internal class ProductViewModel : ValidatedViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }
        public ObservableCollection<ImagePreview<ProductImage>> Images { get; set; }
        public ObservableCollection<Category> Categories { get; set; }

        public string AddBtnText
        {
            get => _addBtnText;
            set => SetValue(ref _addBtnText, value);
        }

        private string _addBtnText;

        private readonly Saller _saller;
        private string _name;
        private string _description;
        private Category? _category;
        private decimal _price;
        private decimal _weight;
        private int _count;
        private int _id;

        [Required(ErrorMessage = "Обязательное поле")]
        [MaxLength(20, ErrorMessage = "Длина не более 20 символов")]
        public string Name
        {
            get => _name;
            set
            {
                SetValue(ref _name, value);
                Validate(Name);
                _validFields[0] = true;
            }
        }

        [Required(ErrorMessage = "Описание обязательно для заполнения")]
        [StringLength(400, ErrorMessage = "Длина описания не должна превышать 400 символов")]
        public string Description
        {
            get => _description;
            set
            {
                SetValue(ref _description, value);
                Validate(Description);
                _validFields[1] = true;
            }
        }

        [Required(ErrorMessage = "Категория обязательна для выбора")]
        public Category Category
        {
            get => _category;
            set
            {
                SetValue(ref _category, value);
                _validFields[2] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                SetValue(ref _price, value);
                _validFields[3] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        private string? _priceStr;

        public string PriceStr
        {
            get => _priceStr ?? "";
            set
            {
                if (value.Contains('.')) value = value.Replace('.', ',');
                string firstPart = "";
                if (value.Length >= 2) firstPart = value.Substring(0, 2);
                if (decimal.TryParse(value, out var result) && !Regex.IsMatch(firstPart, @"^0\d$"))
                {
                    (bool, string, IReadOnlyList<string>) res = result < (decimal)0.1 || result > (decimal)1000000.0
                        ? (false, nameof(PriceStr), ["Неверный диапазон цены"])
                        : (true, nameof(PriceStr), [""]);

                    if (value.Contains(',') && value.IndexOf(',') != value.Length - 3)
                        res = (false, nameof(PriceStr), ["Нужно 2 знака после запятой"]);

                    SetValidationResults(res.Item1, res.Item2, res.Item3);

                    Price = result;
                }
                else SetValidationResults(false, nameof(PriceStr), ["Неверный формат"]);

                _priceStr = value;
                OnPropertyChanged(nameof(PriceStr));
            }
        }


        public decimal Weight
        {
            get => _weight;
            set
            {
                SetValue(ref _weight, value);
                _validFields[4] = true;
            }
        }

        private string? _weightStr;

        public string WeightStr
        {
            get => _weightStr ?? "";
            set
            {
                if (value.Contains('.')) value = value.Replace('.', ',');
                string firstPart = "";
                if (value.Length >= 2) firstPart = value.Substring(0, 2);
                if (decimal.TryParse(value, out var result) && !Regex.IsMatch(firstPart, @"^0\d$"))
                {
                    (bool, string, IReadOnlyList<string>) res = result < (decimal)0.1 || result > (decimal)200.0
                        ? (false, nameof(WeightStr), ["Диапазон от 0.01 до 200.00"])
                        : (true, nameof(WeightStr), [""]);

                    if (value.Contains(',') && value.IndexOf(',') != value.Length - 3)
                        res = (false, nameof(WeightStr), ["Нужно 2 знака после запятой"]);

                    SetValidationResults(res.Item1, res.Item2, res.Item3);

                    Weight = result;
                }
                else SetValidationResults(false, nameof(WeightStr), ["Неверный формат"]);

                _weightStr = value;
                OnPropertyChanged(nameof(WeightStr));
            }
        }

        [Required(ErrorMessage = "Количество обязательно для заполнения")]
        [Range(0, int.MaxValue, ErrorMessage = "Количество не может быть отрицательным")]
        public int Count
        {
            get => _count;
            set
            {
                SetValue(ref _count, value);
                Validate(Count);
            }
        }

        private bool _isWideView;

        public bool IsWideView
        {
            get => _isWideView;
            set
            {
                if (_isWideView != value)
                {
                    _isWideView = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isWidePanel;

        public bool IsWidePanel
        {
            get => _isWidePanel;
            set
            {
                if (_isWidePanel != value)
                {
                    _isWidePanel = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _imageBlocker;

        public bool ImageBlocker
        {
            get => _imageBlocker;
            set => SetValue(ref _imageBlocker, value);
        }

        private string _loadingMsg;

        public string LoadingMsg
        {
            get => _loadingMsg;
            set { this.SetValue(ref _loadingMsg, value); }
        }

        public ICommand AddImage { get; }

        private bool _dataLoaded;

        public bool DataLoaded
        {
            get => _dataLoaded;
            set => SetValue(ref _dataLoaded, value);
        }

        private int _imageCounter;

        public int ImageCounter
        {
            get => _imageCounter;
            set => SetValue(ref _imageCounter, value);
        }


        public ICommand BackCommand { get; }

        public ICommand RegisterCommand { get; }

        public ProductViewModel(Product? product, Saller saller, Shop shop)
        {
            ImageBlocker = true;

            _validFields = new bool[6];
            _loadingMsg = "Загрузка";
            _dataLoaded = true;

            _validFields[2] = true;

            _saller = saller;

            using Repo repo = new();

            Categories = new(repo.GetCategories().Cast<Category>().Where(c => c.ParentId == _saller.CategoryId));

            if (product == null)
            {
                _addBtnText = "Добавить продукт";
                _name = string.Empty;
                _description = string.Empty;
                _count = 1;
                _category = null;
                Images = [];
            }
            else
            {
                _addBtnText = "Изменить";
                _name = product.Name;
                _description = product.Description;
                _count = product.Count;
                _price = product.Price;
                _priceStr = product.Price.ToString();
                _weightStr = product.Weight.ToString();
                _weight = product.Weight;

                using AppContext appContext = new AppContext();

                Images = [];
                Category = appContext.Categories.Where(c => c.Id == product.CategotyId).Cast<Category>().First();

                var images = appContext.ProductImages.Where(i => i.ProductId == product.Id).ToList();

                images.ForEach(i => Images.Add(new ImagePreview<ProductImage>(i, DeleteImage)));

                _validFields = _validFields.Select(f => f = true).ToArray();
            }

            ImageCounter = Images.Count;
            IsWideView = SystemParameters.PrimaryScreenWidth > 1000;
            IsWideView = SystemParameters.PrimaryScreenWidth > 1200;


            AddImage = new Command(async () =>
            {
                if (Images.Count < 6)
                {
                    var path = await AddImageLogik();
                    if (path != null)
                    {
                        ProductImage image = new() { Path = path };
                        Images.Add(new ImagePreview<ProductImage>(image, DeleteImage));
                    }
                }
            });

            Images.CollectionChanged += (s, e) =>
            {
                ImageBlocker = Images.Count < 6;
                _validFields[5] = Images.Count == 6;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
                ImageCounter = Images.Count;
            };

            BackCommand = new Command(() => Navigator?.Navigate(new GoodsPage(_saller)));

            RegisterCommand = new Command(() =>
            {
                Product newProduct = new()
                {
                    Name = _name,
                    Description = _description,
                    CategotyId = _category!.Id,
                    Count = _count,
                    ShopId = shop.Id,
                    Price = _price,
                    Weight = _weight,
                    Images = Images.Select(i => new ProductImage() { Path = i.Path }).ToList(),
                };

                if (product == null)
                {
                    using Repo repo = new();
                    repo.AddProduct(newProduct);
                    _id = newProduct.Id;
                }
                else
                {
                    _id = product.Id;
                    //using Repo repo = new();
                    //repo.ClearProductImage(_id);
                    //repo.RemoveProduct(_id);
                    //repo.AddProduct(newProduct);

                    using AppContext context = new();
                    var pr = context.Products.Find(_id);
                    pr!.Name = _name;
                    pr.Description = _description;
                    pr!.CategotyId = _category!.Id;
                    pr.Count = _count;
                    pr.ShopId = shop.Id;
                    pr.Price = _price;
                    pr.Weight = _weight;
                    pr.Images = Images.Select(i => new ProductImage() { Path = i.Path }).ToList();
                    context.SaveChanges();
                    //_id = shop.Id;
                }
            });
        }

        private void DeleteImage(ProductImage image)
        {
            Images.Remove(Images.Where(i => i.Image == image).First());
        }

        private async Task<string?> AddImageLogik()
        {
            return await Task.Run(() =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Images (*.jpg, *.png)|*.jpg;*.png";
                dialog.AddExtension = true;

                return dialog.ShowDialog() == true ? dialog.FileName : null;
            });
        }
    }
}