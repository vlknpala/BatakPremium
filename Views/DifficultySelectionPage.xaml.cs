using BatakPremium.Models;

namespace BatakPremium.Views;

public partial class DifficultySelectionPage : ContentPage
{
    public DifficultySelectionPage()
    {
        InitializeComponent();
    }

    private void OnPlayClicked(object sender, EventArgs e)
    {
        // Oyna butonunu gizle ve zorluk seviyesi panelini göster
        PlayButton.IsVisible = false;
        DifficultyPanel.IsVisible = true;
    }

    private async void OnDifficultySelected(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            GameDifficulty difficulty = button.Text switch
            {
                "Kolay" => GameDifficulty.Easy,
                "Orta" => GameDifficulty.Medium,
                "Zor" => GameDifficulty.Hard,
                _ => GameDifficulty.Medium
            };

            await Navigation.PushAsync(new MainPage(difficulty));
        }
    }
} 