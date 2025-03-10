using Microsoft.Maui.Graphics;

namespace RandomNumber;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new NavigationPage(new AppShell())
        {
            BarBackgroundColor = Colors.Black, 
            BarTextColor = Color.FromArgb("#007AFF")
        });
    }
}