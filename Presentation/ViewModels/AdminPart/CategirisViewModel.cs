using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Core.Interfaces;
using course_oop.Core.Services;
using course_oop.Infrastructure.Data.Repositories;
using course_oop.Presentation.ViewModels.Base;
using course_oop.Presentation.ViewModels.Commands;
using Microsoft.EntityFrameworkCore;
using Svg;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace course_oop.Presentation.ViewModels.AdminPart
{
    class CategirisViewModel : ValidatedViewModel, INavigated
    {
        private string _newRootCategory;
        private string _newCategory;
        private Category? _salectedCategory;

        public Category? SelectedCategory
        {
            get => _salectedCategory;
            set => SetValue(ref _salectedCategory, value);
        }

        [Required]
        [MaxLength(20, ErrorMessage = "Максимум 20 символов")]
        public string NewRootCategory
        {
            get => _newRootCategory;
            set
            {
                this.SetValue(ref _newRootCategory, value);
                Validate(NewRootCategory);
                _validFields[0] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required]
        [MaxLength(20, ErrorMessage = "Максимум 20 символов")]
        public string NewCategory
        {
            get => _newCategory;
            set
            {
                this.SetValue(ref _newCategory, value);
                Validate(NewCategory);
            }
        }

        public INavigation? Navigator { get; set; }

        public ObservableCollection<Category> Categories { get; set; }

        public ICommand SetRootCommand { get; }

        public ICommand SetCategoryCommand { get; }

        public ICommand DeleteCategoryCommand { get; }

        public ICommand DeleteRootCategoryCommand { get; }

        public CategirisViewModel ()
        {
            using Repo repository = new();

            var categories = repository.GetCategories().Cast<Category>();
            Categories = new(categories.Where(c => c.ParentId == null));
            _validFields = new bool[1];
            _newRootCategory = "";
            _newCategory = "";

            SetRootCommand = new Command(() =>
            {
                Category category = new()
                {
                    Name = _newRootCategory,
                };

                var validResults = ValidationServis.ValidateCategory(category, nameof(NewRootCategory));

                foreach (var result in validResults)
                {
                    SetValidationResults(result.result, result.property, result.message ?? []);
                }

                if (!HasErrors)
                {
                    using Repo repo = new();

                    repo.AddCategory(category);
                    Categories.Add(category);
                }
                NewRootCategory = string.Empty;
                SetValidationResults(true, nameof(NewRootCategory), []);
            });

            SetCategoryCommand = new Command(() =>
            {
                var category = SelectedCategory;

                if (category == null)
                {
                    SetValidationResults(false, nameof(NewCategory), ["Выберете категорию для добавления"]);
                    return;
                }

                var newSubCategory = new Category()
                {
                    Name = _newCategory,
                    ParentId = category.Id,
                };

                var validResults = ValidationServis.ValidateCategory(newSubCategory, nameof(NewCategory));

                foreach (var result in validResults)
                {
                    SetValidationResults(result.result, result.property, result.message ?? []);
                }

                if (!HasErrors)
                {
                    using Repo repo = new();

                    repo.AddCategory(newSubCategory);
                    ChangeCollection(repo.GetCategories().Cast<Category>().Where(c => c.ParentId == null));
                }

                NewCategory = string.Empty;
                SetValidationResults(true, nameof(NewCategory), []);
            });

            DeleteCategoryCommand = new Command<Category>(category =>
            {
                DeleteSubCategory(category);
            });

            DeleteRootCategoryCommand = new Command<Category>(category =>
            {
                

                using AppContext context = new();

                var subcategories = context.Categories.Where(c => c.ParentId == category.Id).Include(c => c.Parent).ToList();

                foreach (var cat in subcategories)
                {
                    DeleteSubCategory(cat, context);
                }

                var shop = context.Shops.FirstOrDefault(s => s.Saller.CategoryId == category.Id);

                if (shop != null)
                    context.Shops.Remove(shop);

               context.Sallers.Where(s => s.CategoryId == category.Id)
                .ExecuteUpdate(s => s.SetProperty(s => s.Banned, true)
                  .SetProperty(s => s.CategoryId, (int?)null)
                  );

                context.Categories.Remove(context.Categories.Find(category.Id)!);

                context.SaveChanges();

                Categories.Remove(category);
            });
        }


        private void DeleteSubCategory (Category category, AppContext? appcontext = null)
        {
            foreach (var root in Categories)
            {
                if (root.Children.Any(c => category.Id == c.Id))
                {
                    root.Children.Remove(category);
                    using Repo repo = new();

                    AppContext context = appcontext == null ? new() : appcontext;
                    var removedCategory = context.Categories.Find(category.Id)!;


                    List<int?> nums = [];

                    context.Orders.Where(o => o.Product!.CategotyId == removedCategory.Id).ToList().ForEach(i => nums.Add(i.CourierId));


                    context.Orders.Where(o => o.Product!.CategotyId == removedCategory.Id)
                    .ExecuteUpdate(o => o
                        .SetProperty(x => x.Status, OrderStatus.Rejected)
                        .SetProperty(x => x.CourierId, (int?)null)
                      );

                    double weight = (double)context.Products.Where(p => p.CategotyId == removedCategory.Id).Sum(p => p.Weight);

                    context.Couriers.Where(p => nums.Contains(p.Id))
                        .ExecuteUpdate(c => c.
                        SetProperty(c => c.SallerMinutes, (int?)null)
                        .SetProperty(c => c.UserMinutes, (int?)null)
                        .SetProperty(c => c.CurrentWeight, c => c.CurrentWeight - weight)
                        .SetProperty(c => c.IsWork, false));

                    var productsInCategory = context.Products
                        .Where(p => p.CategotyId == removedCategory.Id)
                        .Include(p => p.Reviews) // Важно: подгружаем отзывы
                        .ToList();

                    // Собираем ВСЕ отзывы этих продуктов в один список
                    var allReviewsToDelete = productsInCategory
                        .SelectMany(p => p.Reviews) // "Разворачиваем" коллекции отзывов
                        .ToList();

                    // Удаляем все отзывы разом
                    context.Rewiews.RemoveRange(allReviewsToDelete);

                    context.Products.RemoveRange(context.Products.Where(p => p.CategotyId == removedCategory.Id));

                    context.Categories.Remove(removedCategory);

                    context.SaveChanges();

                    ChangeCollection(repo.GetCategories().Cast<Category>().Where(c => c.ParentId == null));

                    if(appcontext == null) context.Dispose();
                    break;
                }
            }
        }

        private void ChangeCollection ( IEnumerable<Category> values )
        {
            Categories.Clear();

            foreach (var value in values)
            {
                Categories.Add(value);
            }
        }
    }
}