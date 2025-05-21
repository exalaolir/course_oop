using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
using Microsoft.EntityFrameworkCore.Migrations;

namespace course_oop.Presentation.ViewModels.AdminPart
{
    internal sealed class InvitesViewModel : ViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }

        public ObservableCollection<Saller> Sallers
        {
            get => _sallers;
            set => _sallers = value;
        }

        private ObservableCollection<Saller> _sallers;
        private int _alternationCount;

        public int AlternationCount
        {
            get => _alternationCount;
            set => this.SetValue(ref _alternationCount, value);
        }


        public ICommand Decline { get; }
        public ICommand Accept { get; }

        public SortingService<Saller> SortingService { get; set; }

        public ICommand SortByCategoryCommand { get; }

        public ICommand UpdateCommand { get; }


        public InvitesViewModel()
        {
            using Repo repository = new();
            var sellers = repository.GetUnVishedSealers();

            _sallers = [.. sellers != null ? sellers.Cast<Saller>() : []];

            _sallers.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                AlternationCount = _sallers.Count + 1;
            };

            SortingService = new(ref _sallers, () => this.OnPropertyChanged(nameof(_sallers)));

            Decline = new Command<Saller>(saller =>
            {
                saller.Banned = true;
                using Repo repository = new();
                repository.UpdateEntity(saller.Id, saller, nameof(Saller.Banned), nameof(Saller.Vished));
                _sallers.Remove(saller);
            });

            Accept = new Command<Saller>(saller =>
            {
                saller.Vished = true;
                using Repo repository = new();
                repository.UpdateEntity(saller.Id, saller, nameof(Saller.Banned), nameof(Saller.Vished));
                _sallers.Remove(saller);
            });

            SortByCategoryCommand = new Command(() =>
            {
                SortingService<Saller>.SortCollection(e => e.Category, _sallers);
                OnPropertyChanged(nameof(_sallers));
            });

            UpdateCommand = new Command(() =>
            {
                using Repo repository = new();
                var newData = repository.GetUnVishedSealers();

                _sallers.Clear();

                if (newData != null)
                {
                    foreach (Saller saller in newData)
                        _sallers.Add(saller);
                }
            });
        }
    }
}