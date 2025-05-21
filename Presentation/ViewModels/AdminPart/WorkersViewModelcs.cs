using System;
using System.Collections;
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

namespace course_oop.Presentation.ViewModels.AdminPart
{
    internal class WorkersViewModelcs : ViewModel, INavigated
    {
        public INavigation? Navigator { get; set; }

        public ObservableCollection<object> Entities
        {
            get => _entities;
            set => _entities = value;
        }

        private ObservableCollection<object> _entities;
        private ObservableCollection<User> _users;
        private int _alternationCount;
        private List<object> _dbUsers;
        private List<object> _dbSallers;
        private List<object> _dbCouriers;
        private string? _searchText;

        public string SearchText
        {
            get => _searchText ?? "";
            set { this.SetValue(ref _searchText, value); }
        }


        public SortingService<User> SortingService { get; set; }

        public int AlternationCount
        {
            get => _alternationCount;
            set => this.SetValue(ref _alternationCount, value);
        }

        public ICommand Ban { get; }
        public ICommand SeeProfil { get; }

        public ICommand ChangeType { get; }

        public ICommand UpdateCommand { get; }

        public ICommand SearchCommand { get; }

        public RegisterCourierViewModel Registration { get; set; }

        public WorkersViewModelcs()
        {
            using Repo repository = new();
            _dbSallers = repository.GetVishedSealers().Cast<object>().ToList();
            _dbCouriers = repository.GetNotBannedCouriers().Cast<object>().ToList();
            _dbUsers = repository.GetNotBannedUsers().Cast<object>().ToList();

            _entities = new(_dbUsers
                .Union(_dbSallers)
                .Union(_dbCouriers));

            AlternationCount = _entities.Count + 1;

            Registration = new RegisterCourierViewModel();

            _entities.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
            {
                AlternationCount = _entities.Count + 1;
            };

            _users = new(_entities.Select(e => (User)e));
            SortingService = new(ref _users, () =>
            {
                _entities.Clear();
                foreach (object item in _users)
                {
                    _entities.Add(item);
                }

                OnPropertyChanged(nameof(_entities));
            });

            Ban = new Command<object>(entity =>
            {
                RevriteEntity(entity);
                _entities.Remove(entity);
            });

            UpdateCommand = new Command(() =>
            {
                using Repo repository = new();
                _dbSallers = repository.GetVishedSealers().Cast<object>().ToList();
                _dbCouriers = repository.GetNotBannedCouriers().Cast<object>().ToList();
                _dbUsers = repository.GetNotBannedUsers().Cast<object>().ToList();

                _entities.Clear();

                var newData = _dbUsers
                    .Union(_dbSallers)
                    .Union(_dbCouriers);

                if (newData != null)
                {
                    foreach (object saller in newData)
                        _entities.Add(saller);
                }
            });

            ChangeType = new Command<string>(typeName =>
            {
                IEnumerable? entities = null;

                switch (typeName)
                {
                    case "user":
                        entities = _dbUsers;
                        break;
                    case "saller":
                        entities = _dbSallers;
                        break;
                    case "courier":
                        entities = _dbCouriers;
                        break;
                    case "all":
                        entities = _dbUsers
                            .Union(_dbSallers)
                            .Union(_dbCouriers);
                        break;
                }

                _entities.Clear();
                foreach (var item in entities!)
                {
                    _entities.Add(item);
                }

                _users.Clear();
                foreach (User item in _entities)
                {
                    _users.Add(item);
                }
            });

            SearchCommand = new Command(() =>
            {
                var newData = _dbUsers
                    .Union(_dbSallers)
                    .Union(_dbCouriers);

                _entities.Clear();
                if (newData != null)
                {
                    foreach (object saller in newData)
                        _entities.Add(saller);
                }

                var res = SortingService.Search(_searchText ?? "");

                _entities.Clear();

                foreach (object item in res)
                {
                    _entities.Add(item);
                }
            });
        }

        private void RevriteEntity(object? entity)
        {
            using Repo repository = new();
            switch (entity)
            {
                case Saller saller:
                    saller.Banned = true;
                    repository.UpdateEntity(saller.Id, saller, nameof(Saller.Banned));
                    break;

                case Courier courier:
                    courier.Banned = true;
                    repository.UpdateEntity(courier.Id, courier, nameof(Courier.Banned));
                    break;

                case User user:
                    user.Banned = true;
                    repository.UpdateEntity(user.Id, user, nameof(User.Banned));
                    break;
            }
        }
    }
}