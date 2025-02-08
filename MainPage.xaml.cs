using BatakPremium.Models;

namespace BatakPremium;

public partial class MainPage : ContentPage
{
    private Game game = null!;
    private readonly GameDifficulty difficulty;
    private Card? selectedCard;

    public MainPage(GameDifficulty difficulty)
    {
        InitializeComponent();
        this.difficulty = difficulty;
        StartNewGame();

        // Kart seçim efekti için event handler'ları ekle
        TopRowCards.SelectionChanged += OnCardSelectionChanged;
        BottomRowCards.SelectionChanged += OnCardSelectionChanged;
    }

    private void OnCardSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Önceki seçili kartı sıfırla
        if (selectedCard != null)
        {
            selectedCard.IsSelected = false;
        }

        // Yeni seçili kartı güncelle
        if (e.CurrentSelection.FirstOrDefault() is Card card)
        {
            selectedCard = card;
            card.IsSelected = true;

            // Diğer CollectionView'ın seçimini temizle
            if (sender == TopRowCards)
                BottomRowCards.SelectedItem = null;
            else
                TopRowCards.SelectedItem = null;

            // Kartı otomatik olarak oyna
            PlaySelectedCard();
        }
    }

    private async void PlaySelectedCard()
    {
        if (selectedCard != null)
        {
            bool played = game.PlayCard(selectedCard);
            if (played)
            {
                selectedCard.IsSelected = false;
                selectedCard = null;

                // UI'ı güncelle
                UpdateUI();

                // Bilgisayar hamlelerini yap
                await game.PlayComputerTurns();
                UpdateUI();

                // Oyun bitti mi kontrol et
                if (game.PlayerCards.Count == 0)
                {
                    await ShowGameResults();
                }
            }
        }
    }

    private void StartNewGame()
    {
        game = new Game(difficulty);
        UpdateUI();
    }

    private void UpdateUI()
    {
        var cards = game.PlayerCards.ToList();
        var halfCount = (cards.Count + 1) / 2;

        // Oyuncu kartlarını güncelle
        TopRowCards.ItemsSource = cards.Take(halfCount);
        BottomRowCards.ItemsSource = cards.Skip(halfCount);

        // Skor etiketlerini güncelle
        PlayerScoreLabel.Text = game.PlayerScore.ToString();
        Computer1ScoreLabel.Text = game.Computer1Score.ToString();
        Computer2ScoreLabel.Text = game.Computer2Score.ToString();
        Computer3ScoreLabel.Text = game.Computer3Score.ToString();

        // Masadaki kartları güncelle
        UpdateTableCards();

        // Bilgisayar kartlarını güncelle
        Computer1Cards.ItemsSource = game.ComputerCards1;
        Computer2Cards.ItemsSource = game.ComputerCards2;
        Computer3Cards.ItemsSource = game.ComputerCards3;
    }

    private void UpdateTableCards()
    {
        var tableCards = game.TableCards;
        
        // Tüm kart frame'lerini gizle
        TopPlayerCard.IsVisible = false;
        LeftPlayerCard.IsVisible = false;
        RightPlayerCard.IsVisible = false;
        BottomPlayerCard.IsVisible = false;

        // Masadaki kartları göster
        for (int i = 0; i < tableCards.Count; i++)
        {
            var card = tableCards[i];
            switch (i)
            {
                case 0: // Alt oyuncu (insan oyuncu)
                    BottomPlayerCard.IsVisible = true;
                    BottomPlayerCard.BindingContext = card;
                    break;
                case 1: // Sol bilgisayar
                    LeftPlayerCard.IsVisible = true;
                    LeftPlayerCard.BindingContext = card;
                    break;
                case 2: // Üst bilgisayar
                    TopPlayerCard.IsVisible = true;
                    TopPlayerCard.BindingContext = card;
                    break;
                case 3: // Sağ bilgisayar
                    RightPlayerCard.IsVisible = true;
                    RightPlayerCard.BindingContext = card;
                    break;
            }
        }
    }

    private async Task ShowGameResults()
    {
        string message = "Oyun Bitti!\n\n" +
                        $"Sizin Skorunuz: {game.PlayerScore}\n" +
                        $"1. Oyuncu: {game.Computer3Score}\n" +
                        $"2. Oyuncu: {game.Computer2Score}\n" +
                        $"3. Oyuncu: {game.Computer1Score}";

        bool playAgain = await DisplayAlert("Oyun Bitti", message, "Yeni Oyun", "Çıkış");
        
        if (playAgain)
        {
            StartNewGame();
        }
        else
        {
            await Navigation.PopAsync();
        }
    }
}
