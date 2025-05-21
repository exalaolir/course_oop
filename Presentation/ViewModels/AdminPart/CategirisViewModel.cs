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

        public CategirisViewModel()
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
            });

            DeleteCategoryCommand = new Command<Category>(category =>
            {
                foreach (var root in Categories)
                {
                    if (root.Children.Contains(category))
                    {
                        root.Children.Remove(category);
                        using Repo repo = new();

                        repo.RemoveCategory(category);
                        ChangeCollection(repo.GetCategories().Cast<Category>().Where(c => c.ParentId == null));
                        break;
                    }
                }
            });

            DeleteRootCategoryCommand = new Command<Category>(category =>
            {
                Categories.Remove(category);
                using Repo repo = new();

                repo.RemoveCategory(category);
            });
        }

        private void ChangeCollection(IEnumerable<Category> values)
        {
            Categories.Clear();

            foreach (var value in values)
            {
                Categories.Add(value);
            }
        }
    }
}