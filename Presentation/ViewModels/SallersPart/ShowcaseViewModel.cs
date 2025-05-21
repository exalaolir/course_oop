using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Core.Services;
using course_oop.Infrastructure.Data.Repositories;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using course_oop.Presentation.ViewModels.Components;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;

namespace course_oop.Presentation.ViewModels.SallersPart
{
    class ShowcaseViewModel : ValidatedViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }

        private List<ImagePreview<Image>>? _images;

        [Required]
        [MaxLength(30)]
        public string Name
        {
            get => _name;
            set
            {
                SetValue(ref _name, value);
                Validate(Name);
                _validFields[0] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required]
        [MaxLength(200)]
        public string Description
        {
            get => _description;
            set
            {
                SetValue(ref _description, value);
                Validate(Description);
                _validFields[1] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required]
        public string Adress
        {
            get => _adreess;
            set
            {
                SetValue(ref _adreess, value);
                Validate(Adress);
                _validFields[2] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        private Saller _saller;

        private string _name;
        private string _description;
        private string _adreess;

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

        public ObservableCollection<ImagePreview<Image>> Images { get; set; }

        public ICommand AddImage { get; }

        public void ClearImages()
        {
            _images = new(Images);
            Images.Clear();
        }

        public void RerenderImages()
        {
            if (_images != null)
            {
                foreach (var image in _images)
                {
                    Images.Add(image);
                }
            }
        }

        private bool _dataLoaded;

        private int _imageCounter;

        public int ImageCounter
        {
            get => _imageCounter;
            set => SetValue(ref _imageCounter, value);
        }

        private bool _imageBlocker;

        public bool ImageBlocker
        {
            get => _imageBlocker;
            set => SetValue(ref _imageBlocker, value);
        }

        public string AddBtnText
        {
            get => _addBtnText;
            set => SetValue(ref _addBtnText, value);
        }

        private string _addBtnText;
        private string _loadingMsg;

        public string LoadingMsg
        {
            get => _loadingMsg;
            set { this.SetValue(ref _loadingMsg, value); }
        }

        private int _id;

        public ICommand SetShopsData { get; }

        public bool DataLoaded
        {
            get => _dataLoaded;
            set => SetValue(ref _dataLoaded, value);
        }

        public ShowcaseViewModel(Saller saller)
        {
            ImageBlocker = true;

            _validFields = new bool[4];
            _loadingMsg = "Загрузка";
            _dataLoaded = true;

            _saller = saller;
            using Repo repo = new();
            var shop = repo.GetShop(saller.Id).Cast<Shop>().FirstOrDefault();
            if (shop == null)
            {
                _name = string.Empty;
                _description = string.Empty;
                _adreess = string.Empty;
                _addBtnText = "Создать магазин";
                Images = [];
            }
            else
            {
                _addBtnText = "Сохранить изменения";
                _name = shop.Name;
                _adreess = shop.Adress ?? "";
                _description = shop.Description ?? "";
                var images = repo.GetImages(shop.Id)
                    .Cast<Image>().ToList();
                Images = [];
                images.ForEach(i => Images.Add(new ImagePreview<Image>(i, DeleteImage)));
                _validFields = _validFields.Select(e => e = true).ToArray();
                _id = shop.Id;
                ImageBlocker = false;
            }

            ImageCounter = Images.Count;

            AddImage = new Command(async () =>
            {
                if (Images.Count < 6)
                {
                    var path = await AddImageLogik();
                    if (path != null)
                    {
                        Image image = new() { Path = path };
                        Images.Add(new ImagePreview<Image>(image, DeleteImage));
                    }
                }
            });

            IsWideView = SystemParameters.PrimaryScreenWidth > 1000;
            IsWideView = SystemParameters.PrimaryScreenWidth > 1200;

            Images.CollectionChanged += (s, e) =>
            {
                ImageBlocker = Images.Count >= 6;
                _validFields[3] = Images.Count == 6;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
                ImageCounter = Images.Count;
            };

            SetShopsData = new Command(async () =>
            {
                try
                {
                    DataLoaded = false;
                    var adress = await MapServise.GetCoordsByQuery(Adress);
                    DataLoaded = true;

                    Shop shop = new()
                    {
                        Name = Name,
                        Description = Description,
                        X = adress.x,
                        Y = adress.y,
                        SallerId = _saller.Id,
                        Adress = adress.adress,
                        Images = Images.Select(x => new Image()
                        {
                            Path = x.Path,
                        }).ToList()
                    };


                    var validResults = ValidationServis.ValidateShop(shop);

                    foreach (var result in validResults)
                    {
                        SetValidationResults(result.result, result.property, result.message ?? []);
                    }

                    if (!HasErrors)
                    {
                        if (_addBtnText == "Создать магазин")
                        {
                            using Repo repo = new();
                            repo.AddShop(shop);

                            new ToastContentBuilder()
                                .AddArgument("action", "viewConversation")
                                .AddArgument("conversationId", 9813)
                                .AddText("Успех")
                                .AddText("Магазин добавлен")
                                .Show();

                            AddBtnText = "Сохранить изменения";
                            _id = shop.Id;
                        }
                        else
                        {
                            using Repo repo = new();
                            repo.ClearImage(_id);
                            repo.RemoveShop(_id);
                            repo.AddShop(shop);
                            _id = shop.Id;
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    LoadingMsg = ex.Message;
                }
                catch (Exception ex)
                {
                    SetValidationResults(false, nameof(Adress), [ex.Message]);
                }
            });
        }

        private void DeleteImage(Image image)
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