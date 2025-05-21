using System.Collections;
using course_oop.Core.Entities;
using Microsoft.IdentityModel.Tokens;

namespace course_oop.Infrastructure.Data.Repositories
{
    internal class Repo : IDisposable
    {
        private readonly AppContext _appContext;

        public Repo()
        {
            _appContext = new();
            if (!_appContext.Database.CanConnect())
                throw new Exception();
        }

        internal void AddCategory(Category category)
        {
            _appContext.Categories.Add(category);
            _appContext.SaveChanges();
        }

        internal IEnumerable GetCategories() => _appContext.Categories;
        internal IEnumerable GetAllUsers() => _appContext.Users;

        internal IEnumerable GetAllProducts() => _appContext.Products;

        internal IQueryable GetShop(int id) => _appContext.Shops.Where(e => e.SallerId == id);

        internal IEnumerable GetShops() => _appContext.Shops;
        internal IQueryable GetImages(int id) => _appContext.Images.Where(e => e.ShopId == id);

        internal IEnumerable GetSealers() => _appContext.Sallers;

        internal IEnumerable GetNotBannedUsers() => _appContext.Users.Where(e => !e.Banned);

        internal IEnumerable GetVishedSealers() => _appContext.Sallers.Where(e => e.Vished);

        internal IEnumerable GetCouriers() => _appContext.Couriers;

        internal IEnumerable GetAdmins() => _appContext.Admins;

        internal IEnumerable GetNotBannedCouriers() => _appContext.Couriers.Where(e => !e.Banned);

        internal IEnumerable GetProducts(int shopId) => _appContext.Products.Where(p => p.ShopId == shopId);

        internal IEnumerable? GetUnVishedSealers() => _appContext.Sallers.Where(e => !e.Vished && !e.Banned);

        internal User? FindUserByEmail(string email)
        {
            var user = _appContext.Users.Where(e => e.Email == email);
            return user.IsNullOrEmpty() ? null : user.First();
        }

        internal Saller? FindSallerByEmail(string email)
        {
            var user = _appContext.Sallers.Where(e => e.Email == email);
            return user.IsNullOrEmpty() ? null : user.First();
        }

        internal Admin? FindAdminByEmail(string email)
        {
            var user = _appContext.Admins.Where(e => e.Email == email);
            return user.IsNullOrEmpty() ? null : user.First();
        }

        internal Courier? FindCurierByEmail(string email)
        {
            var user = _appContext.Couriers.Where(e => e.Email == email);
            return user.IsNullOrEmpty() ? null : user.First();
        }

        internal void ClearImage(int id)
        {
            _appContext.Images.RemoveRange(_appContext.Images.Where(i => i.ShopId == id));
            _appContext.SaveChanges();
        }

        internal void ClearProductImage(int id)
        {
            _appContext.ProductImages.RemoveRange(_appContext.ProductImages.Where(i => i.ProductId == id));
            _appContext.SaveChanges();
        }

        internal void AddUser(User user)
        {
            _appContext.Users.Add(user);
            _appContext.SaveChanges();
        }

        internal void AddShop(Shop shop)
        {
            _appContext.Shops.Add(shop);
            _appContext.SaveChanges();
        }

        internal void AddSaller(Saller saller)
        {
            _appContext.Sallers.Add(saller);
            _appContext.SaveChanges();
        }

        internal void AddCourier(Courier saller)
        {
            _appContext.Couriers.Add(saller);
            _appContext.SaveChanges();
        }

        internal void AddProduct(Product product)
        {
            _appContext.Products.Add(product);
            _appContext.SaveChanges();
        }

        internal void RemoveCategory(Category category)
        {
            if (category.ParentId != null)
            {
                _appContext.Categories.Remove(category);
                _appContext.Products.RemoveRange(_appContext.Products.Where(p => p.CategotyId == category.Id));
            }
            else
            {
                _appContext.Categories.RemoveRange(category.Children);
                foreach (Category child in category.Children)
                {
                    _appContext.Products.RemoveRange(_appContext.Products.Where(p => p.CategotyId == child.Id));
                }

                //_appContext.Shops.RemoveRange(_appContext.Shops.Where(s => s.));
                _appContext.Categories.Remove(category);
            }

            _appContext.SaveChanges();
        }

        internal void RemoveShop(int id)
        {
            _appContext.Shops.Remove(_appContext.Shops.Where(s => s.Id == id).First());
            _appContext.SaveChanges();
        }

        internal void RemoveProduct(int id)
        {
            _appContext.Products.Remove(_appContext.Products.Where(s => s.Id == id).First());
            _appContext.SaveChanges();
        }

        public void UpdateEntity<TEntity>(int id, TEntity updatedEntity, params string[] propertyNames)
            where TEntity : class
        {
            if (updatedEntity == null)
                throw new ArgumentNullException(nameof(updatedEntity));

            var dbEntity = _appContext.Set<TEntity>().Find(id) ??
                           throw new ArgumentNullException($"Сущность {typeof(TEntity).Name} с id {id} не найдена");

            var entityType = typeof(TEntity);
            var properties = propertyNames.Length == 0
                ? entityType.GetProperties()
                : entityType.GetProperties().Where(p => propertyNames.Contains(p.Name));

            if (!properties.Any())
                throw new ArgumentException("Не указаны свойства для обновления");

            foreach (var property in properties)
            {
                var newValue = property.GetValue(updatedEntity);
                property.SetValue(dbEntity, newValue);
            }

            _appContext.SaveChanges();
        }

        public void Dispose() => _appContext.Dispose();
    }
}