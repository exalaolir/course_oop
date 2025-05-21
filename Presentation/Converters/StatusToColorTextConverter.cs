using course_oop.Core.Entities;
using course_oop.Presentation.ViewModels.SallersPart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace course_oop.Presentation.Converters
{
    internal class StatusToColorTextConverter : IValueConverter
    {
        public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if (value is not OrderStatus status)
                return GetStatusData("Неизвестно", "SystemFillColorCriticalBrush", parameter);

            return status switch
            {
                OrderStatus.Processing => GetStatusData("Обработка", "SystemFillColorCautionBrush", parameter),
                OrderStatus.InCart => GetStatusData("В корзине", "SystemFillColorNeutralBrush", parameter),
                OrderStatus.ReadyForCourier => GetStatusData("Обработка", "SystemFillColorCautionBrush", parameter),
                OrderStatus.Rejected => GetStatusData("Заблокирован", "SystemFillColorCriticalBrush", parameter),
                OrderStatus.WaitCourier => GetStatusData("Доставка", "SystemFillColorCautionBrush", parameter),
                OrderStatus.InDelivery => GetStatusData("Доставка", "SystemFillColorCautionBrush", parameter),
                OrderStatus.Delivered => GetStatusData("Доставлен", "SystemFillColorSuccessBrush", parameter),
                _ => GetStatusData("Неизвестно", "SystemFillColorCriticalBrush", parameter)
            };
        }

        private object GetStatusData ( string text, string resourceName, object parameter )
        {
            // Если параметр равен "Text" - возвращаем только текст
            if (parameter is string strParam && strParam == "Text")
                return text;

            // Если параметр равен "Brush" - возвращаем только кисть
            if (parameter is string strParam2 && strParam2 == "Brush")
                return Application.Current.TryFindResource(resourceName) ?? Brushes.Black;

            // По умолчанию возвращаем кортеж
            return new Tuple<string, Brush>(text,
                (Brush)(Application.Current.TryFindResource(resourceName) ?? Brushes.Black));
        }


        public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
