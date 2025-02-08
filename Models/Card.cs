using System.ComponentModel;
using System.Windows.Input;

namespace BatakPremium.Models
{
    public class Card : INotifyPropertyChanged
    {
        public string Suit { get; set; }  // Kağıt türü (Kupa, Karo, Maça, Sinek)
        public string Value { get; set; }  // Kağıt değeri (As, 2, 3, ..., J, Q, K)
        public int Score { get; set; }     // Puanlama için skor değeri
        public string ImagePath { get; set; } // Kart görseli için path

        private double rotationAngle;
        public double RotationAngle
        {
            get => rotationAngle;
            set
            {
                if (rotationAngle != value)
                {
                    rotationAngle = value;
                    OnPropertyChanged(nameof(RotationAngle));
                }
            }
        }

        private double translationY;
        public double TranslationY
        {
            get => translationY;
            set
            {
                if (translationY != value)
                {
                    translationY = value;
                    OnPropertyChanged(nameof(TranslationY));
                }
            }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    TranslationY = value ? -20 : 0;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public ICommand SelectCardCommand { get; }

        public Card(string suit, string value, int score)
        {
            Suit = suit;
            Value = value;
            Score = score;
            ImagePath = $"card_{suit.ToLower()}_{value.ToLower()}.png";
            RotationAngle = 0;
            TranslationY = 0;
            IsSelected = false;
            SelectCardCommand = new Command(() => IsSelected = !IsSelected);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 