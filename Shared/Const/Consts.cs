namespace course_oop.Shared.Const
{
    public static class Consts
    {
        public enum Roles
        {
            User,
            Saler,
            Admin,
            Courier
        }

        public enum AuthResults
        {
            Success,
            PasswordError,
            EmailError
        }

        public enum Transport
        {
            Foot,
            Bycke,
            Car
        }

        public const string DbConnection =
            @"Server=DESKTOP-1OQH158;Database=delivery_db;Trusted_Connection=True;TrustServerCertificate=True;" +
            "Pooling=true;" +
            "Min Pool Size=5;" +
            "Max Pool Size=100;" +
            "Connection Lifetime=300;" +
            "Connection Timeout=30;";

        public const int ValidatorCapacity = 10;

        public static readonly IReadOnlyList<string> EmailMessage = ["Email занят"];

        public static readonly IReadOnlyList<string> PhoneMessage = ["Телефон занят"];

        public static readonly IReadOnlyList<string> CategoryMessage = ["Категория занята"];

        public static readonly IReadOnlyList<string> SallerIdMessage = ["Данный идентификатор уже зарегистрирован"];

        public static readonly IReadOnlyList<string> AdressMessage = ["Адрес уже занят"];

        public static readonly IReadOnlyList<string> ShopNameMessageMessage = ["Имя магазина занято"];
    }
}