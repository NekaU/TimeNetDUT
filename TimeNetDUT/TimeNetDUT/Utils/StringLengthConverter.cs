using System;
using Xamarin.Forms;

// Определяем пространство имен, в котором находится класс StringLengthConverter
namespace TimeNetDUT.Utils
{
    // Объявляем класс StringLengthConverter, который реализует интерфейс IValueConverter
    public class StringLengthConverter : IValueConverter
    {
        // Реализация метода Convert, который преобразует значение типа string в значение типа double
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string str) // Проверяем, является ли value типом string
            {
                return str.Length * 14; // Возвращаем значение, равное длине строки, умноженной на размер шрифта (в данном случае 14)
            }
            return 0; // Если value не является типом string, то возвращаем 0
        }

        // Реализация метода ConvertBack, который не используется в данном примере
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException(); // Выбрасываем исключение NotImplementedException, так как метод не реализован
        }
    }

}

