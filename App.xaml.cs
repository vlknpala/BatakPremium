using BatakPremium.Views;

namespace BatakPremium;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new NavigationPage(new DifficultySelectionPage())
        {
            BarBackgroundColor = Color.FromArgb("#1B4D3E"),
            BarTextColor = Colors.White
        };
    }
}
