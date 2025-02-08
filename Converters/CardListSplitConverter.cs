using System.Collections;
using System.Globalization;
using BatakPremium.Models;

namespace BatakPremium.Converters
{
    public class CardListSplitConverter : IValueConverter
    {
        public string Position { get; set; } = "First"; // "First" or "Second"

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is IEnumerable<Card> cards)
            {
                var cardList = cards.ToList();
                var halfCount = (cardList.Count + 1) / 2; // Upper row may have one extra card
                return Position == "First" ? cardList.Take(halfCount) : cardList.Skip(halfCount);
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 