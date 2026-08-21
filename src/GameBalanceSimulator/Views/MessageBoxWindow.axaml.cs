using Avalonia.Controls;
using Avalonia.Interactivity;

namespace GameBalanceSimulator.Views;

public partial class MessageBoxWindow : Window
{
    public MessageBoxWindow()
    {
        InitializeComponent();
    }

    public string Message
    {
        get => MessageText.Text ?? string.Empty;
        set => MessageText.Text = value;
    }

    public static async Task ShowInfoAsync(Window owner, string message, string title)
    {
        var window = new MessageBoxWindow
        {
            Title = title,
            Message = message
        };
        window.OkButton.IsVisible = true;
        await window.ShowDialog(owner);
    }

    public static async Task<bool> ShowConfirmAsync(Window owner, string message, string title)
    {
        var window = new MessageBoxWindow
        {
            Title = title,
            Message = message
        };
        window.YesButton.IsVisible = true;
        window.NoButton.IsVisible = true;
        return await window.ShowDialog<bool>(owner);
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e) => Close(true);

    private void YesButton_Click(object? sender, RoutedEventArgs e) => Close(true);

    private void NoButton_Click(object? sender, RoutedEventArgs e) => Close(false);
}
