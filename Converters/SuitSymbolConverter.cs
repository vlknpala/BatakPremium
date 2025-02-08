using System.Globalization;

namespace BatakPremium.Converters
{
    public class SuitSymbolConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string suit)
            {
                return suit switch
                {
                    "Kupa" => "♥",
                    "Karo" => "♦",
                    "Maça" => "♠",
                    "Sinek" => "♣",
                    _ => "?"
                };
            }
            return "?";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 