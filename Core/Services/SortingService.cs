using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using course_oop.Core.Entities;
using course_oop.Presentation.ViewModels.Commands;

namespace course_oop.Core.Services
{
    internal sealed class SearchData(bool isSelect, string field)
    {
        public bool IsSelect { get; set; } = isSelect;
        public string Field { get; set; } = field;
    }

    internal class SortingService<TValue> where TValue : User
    {
        private readonly ObservableCollection<TValue> _collection;

        private readonly Action _action;

        public ObservableCollection<SearchData> SearchFields { get; set; }

        public ICommand SortByPhoneCommand { get; }
        public ICommand SortByEmailCommand { get; }
        public ICommand SortByNameCommand { get; }
        public ICommand SortByFirstNameCommand { get; }

        public SortingService(ref ObservableCollection<TValue> collection, Action action)
        {
            _collection = collection;
            _action = action;

            SearchFields =
            [
                new(false, nameof(User.Name)),
                new(false, nameof(User.FirstName)),
                new(false, nameof(User.Email)),
                new(false, nameof(User.Phone)),
            ];

            SortByPhoneCommand = new Command(() => SortCollection(e => e.Phone));
            SortByEmailCommand = new Command(() => SortCollection(e => e.Email));
            SortByNameCommand = new Command(() => SortCollection(e => e.Name));
            SortByFirstNameCommand = new Command(() => SortCollection(e => e.FirstName));
        }

        internal void SortCollection<TKey>(Func<TValue, TKey> keySelector)
        {
            var sorted = _collection.OrderBy(keySelector).ToList();

            _collection.Clear();
            foreach (var item in sorted)
            {
                _collection.Add(item);
            }

            _action.Invoke();
        }

        internal IEnumerable<TValue> Search(string text)
        {
            return _collection.Where(e =>
            {
                var props = e.GetType();
                foreach (var field in SearchFields.Where(e => e.IsSelect))
                {
                    var val = props.GetProperty(field.Field)!.GetValue(e);
                    if (Regex.IsMatch(val?.ToString() ?? "", $"^{Regex.Escape(text)}"))
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        internal static void SortCollection<TKey, TVal>(Func<TVal, TKey> keySelector,
            ObservableCollection<TVal> collection) where TVal : class
        {
            var sorted = collection.OrderBy(keySelector).ToList();

            collection.Clear();
            foreach (var item in sorted)
            {
                collection.Add(item);
            }
        }
    }
}