using System.Collections.ObjectModel;

namespace BatakPremium.Models
{
    public class Game
    {
        public ObservableCollection<Card> PlayerCards { get; set; }
        public ObservableCollection<Card> ComputerCards1 { get; set; }
        public ObservableCollection<Card> ComputerCards2 { get; set; }
        public ObservableCollection<Card> ComputerCards3 { get; set; }
        public ObservableCollection<Card> TableCards { get; set; }
        
        public int PlayerScore { get; set; }
        public int Computer1Score { get; set; }
        public int Computer2Score { get; set; }
        public int Computer3Score { get; set; }

        private List<Card> deck = new();
        private string? currentSuit = "Maça"; // Koz her zaman maça
        private bool isPlayerTurn = true;
        private GameDifficulty difficulty;
        private int currentPlayerIndex = 0; // 0: Oyuncu, 1-3: Bilgisayarlar

        public Game(GameDifficulty difficulty)
        {
            this.difficulty = difficulty;
            PlayerCards = new ObservableCollection<Card>();
            ComputerCards1 = new ObservableCollection<Card>();
            ComputerCards2 = new ObservableCollection<Card>();
            ComputerCards3 = new ObservableCollection<Card>();
            TableCards = new ObservableCollection<Card>();
            InitializeDeck();
            DealCards();
        }

        private void InitializeDeck()
        {
            string[] suits = { "Kupa", "Karo", "Maça", "Sinek" };
            string[] values = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
            
            foreach (var suit in suits)
            {
                foreach (var value in values)
                {
                    int score = value switch
                    {
                        "A" => 11,
                        "K" => 4,
                        "Q" => 3,
                        "J" => 2,
                        "10" => 10,
                        _ => 0
                    };
                    deck.Add(new Card(suit, value, score));
                }
            }
        }

        private void DealCards()
        {
            // Kartları karıştır
            var rnd = new Random();
            deck = deck.OrderBy(x => rnd.Next()).ToList();

            // Her oyuncuya 13 kart dağıt
            for (int i = 0; i < 52; i++)
            {
                var card = deck[i];
                int cardIndex = i % 13;
                double angle = -15 + (30.0 * cardIndex / 12); // -15 ile +15 derece arasında yelpaze şeklinde

                card.RotationAngle = angle;

                if (i < 13)
                    PlayerCards.Add(card);
                else if (i < 26)
                    ComputerCards1.Add(card);
                else if (i < 39)
                    ComputerCards2.Add(card);
                else
                    ComputerCards3.Add(card);
            }

            // Oyuncunun kartlarını sırala
            var sortedPlayerCards = PlayerCards.OrderBy(c => c.Suit).ThenBy(c => GetCardValue(c.Value)).ToList();
            PlayerCards.Clear();
            for (int i = 0; i < sortedPlayerCards.Count; i++)
            {
                var card = sortedPlayerCards[i];
                double angle = -15 + (30.0 * i / (sortedPlayerCards.Count - 1)); // Sıralanmış kartlar için yelpaze açısını yeniden hesapla
                card.RotationAngle = angle;
                PlayerCards.Add(card);
            }
        }

        private int GetCardValue(string value)
        {
            return value switch
            {
                "2" => 2,
                "3" => 3,
                "4" => 4,
                "5" => 5,
                "6" => 6,
                "7" => 7,
                "8" => 8,
                "9" => 9,
                "10" => 10,
                "J" => 11,
                "Q" => 12,
                "K" => 13,
                "A" => 14,
                _ => 0
            };
        }

        public bool PlayCard(Card card)
        {
            if (!isPlayerTurn) return false;
            if (!IsValidMove(card)) return false;

            TableCards.Add(card);
            PlayerCards.Remove(card);
            currentPlayerIndex = 1;
            isPlayerTurn = false;
            return true;
        }

        private bool IsValidMove(Card card)
        {
            if (TableCards.Count == 0) return true;

            var firstCard = TableCards[0];
            // Eğer aynı suit'ten kart varsa onu oynamak zorunda
            if (PlayerCards.Any(c => c.Suit == firstCard.Suit))
            {
                return card.Suit == firstCard.Suit;
            }
            return true;
        }

        public async Task PlayComputerTurns()
        {
            while (!isPlayerTurn && TableCards.Count < 4)
            {
                await Task.Delay(1000); // Düşünme efekti
                PlayComputerCard();
                currentPlayerIndex = (currentPlayerIndex + 1) % 4;
                if (currentPlayerIndex == 0)
                {
                    EvaluateRound();
                    isPlayerTurn = true;
                }
            }
        }

        private void PlayComputerCard()
        {
            var computerCards = currentPlayerIndex switch
            {
                1 => ComputerCards1,
                2 => ComputerCards2,
                3 => ComputerCards3,
                _ => throw new InvalidOperationException()
            };

            Card selectedCard = SelectComputerCard(computerCards);
            TableCards.Add(selectedCard);
            computerCards.Remove(selectedCard);
        }

        private Card SelectComputerCard(ObservableCollection<Card> computerCards)
        {
            // Zorluk seviyesine göre kart seçme stratejisi
            switch (difficulty)
            {
                case GameDifficulty.Easy:
                    return SelectEasyCard(computerCards);
                case GameDifficulty.Medium:
                    return SelectMediumCard(computerCards);
                case GameDifficulty.Hard:
                    return SelectHardCard(computerCards);
                default:
                    return computerCards[0];
            }
        }

        private Card SelectEasyCard(ObservableCollection<Card> cards)
        {
            // Basit strateji: Rastgele kart seç
            if (TableCards.Count == 0) return cards[new Random().Next(cards.Count)];

            // İlk kartın suit'i varsa onu oyna
            var firstCard = TableCards[0];
            var validCards = cards.Where(c => c.Suit == firstCard.Suit).ToList();
            if (validCards.Any())
            {
                return validCards[new Random().Next(validCards.Count)];
            }
            return cards[new Random().Next(cards.Count)];
        }

        private Card SelectMediumCard(ObservableCollection<Card> cards)
        {
            if (TableCards.Count == 0)
            {
                // Orta seviye: Düşük puanlı kartları tercih et
                var lowScoreCards = cards.Where(c => c.Score <= 3).ToList();
                if (lowScoreCards.Any())
                {
                    return lowScoreCards[new Random().Next(lowScoreCards.Count)];
                }
            }

            var firstCard = TableCards.FirstOrDefault();
            if (firstCard != null)
            {
                var validCards = cards.Where(c => c.Suit == firstCard.Suit).ToList();
                if (validCards.Any())
                {
                    // Orta seviye: En yüksek puanlı kartı oyna
                    return validCards.OrderByDescending(c => c.Score).First();
                }
            }

            // Koz kullan
            var trumpCards = cards.Where(c => c.Suit == currentSuit).ToList();
            if (trumpCards.Any())
            {
                return trumpCards.OrderBy(c => c.Score).First();
            }

            return cards[new Random().Next(cards.Count)];
        }

        private Card SelectHardCard(ObservableCollection<Card> cards)
        {
            if (TableCards.Count == 0)
            {
                // Zor seviye: Stratejik başlangıç
                var nonTrumpHighCards = cards.Where(c => c.Suit != currentSuit && c.Score >= 10).ToList();
                if (nonTrumpHighCards.Any())
                {
                    return nonTrumpHighCards.OrderByDescending(c => c.Score).First();
                }
            }

            var firstCard = TableCards.FirstOrDefault();
            if (firstCard != null)
            {
                var validCards = cards.Where(c => c.Suit == firstCard.Suit).ToList();
                if (validCards.Any())
                {
                    // El alabilecek en düşük kartı oyna
                    var winningCards = validCards.Where(c => c.Score > TableCards.Max(tc => tc.Score)).ToList();
                    if (winningCards.Any())
                    {
                        return winningCards.OrderBy(c => c.Score).First();
                    }
                    return validCards.OrderBy(c => c.Score).First();
                }

                // Koz kullan
                var trumpCards = cards.Where(c => c.Suit == currentSuit).ToList();
                if (trumpCards.Any() && !TableCards.Any(c => c.Suit == currentSuit))
                {
                    return trumpCards.OrderBy(c => c.Score).First();
                }
            }

            return cards.OrderBy(c => c.Score).First();
        }

        private void EvaluateRound()
        {
            if (TableCards.Count != 4) return;

            var firstCard = TableCards[0];
            var winningCard = TableCards[0];
            var winningPlayerIndex = 0;

            for (int i = 1; i < 4; i++)
            {
                var currentCard = TableCards[i];
                if (currentCard.Suit == currentSuit && winningCard.Suit != currentSuit)
                {
                    winningCard = currentCard;
                    winningPlayerIndex = i;
                }
                else if (currentCard.Suit == winningCard.Suit && currentCard.Score > winningCard.Score)
                {
                    winningCard = currentCard;
                    winningPlayerIndex = i;
                }
            }

            // Puanları güncelle
            int roundScore = TableCards.Sum(c => c.Score);
            switch (winningPlayerIndex)
            {
                case 0:
                    PlayerScore += roundScore;
                    break;
                case 1:
                    Computer1Score += roundScore;
                    break;
                case 2:
                    Computer2Score += roundScore;
                    break;
                case 3:
                    Computer3Score += roundScore;
                    break;
            }

            TableCards.Clear();
            isPlayerTurn = winningPlayerIndex == 0;
            currentPlayerIndex = winningPlayerIndex;
        }
    }
} 