using System.Globalization;

namespace BatakPremium.Converters
{
    public class SuitColorConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string suit)
            {
                return suit switch
                {
                    "Kupa" => Colors.Red,
                    "Karo" => Colors.Red,
                    "Maça" => Colors.Black,
                    "Sinek" => Colors.Black,
                    _ => Colors.Black
                };
            }
            return Colors.Black;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 