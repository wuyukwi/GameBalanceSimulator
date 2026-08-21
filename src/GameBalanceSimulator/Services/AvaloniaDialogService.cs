using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using GameBalanceSimulator.ViewModels.Services;

namespace GameBalanceSimulator.Services;

public sealed class AvaloniaDialogService : IDialogService
{
    private static Window? GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow;
        }

        return null;
    }

    public Task ShowInfoAsync(string message, string? title = null)
    {
        // Placeholder: implement a dedicated message-box window when UI polish is needed.
        return Task.CompletedTask;
    }

    public Task<bool> ShowConfirmAsync(string message, string? title = null)
    {
        // Placeholder: implement a dedicated confirm dialog when UI polish is needed.
        return Task.FromResult(true);
    }

    public async Task<string?> ShowOpenFileAsync(string title, string filter)
    {
        var mainWindow = GetMainWindow();
        if (mainWindow is null)
        {
            return null;
        }

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false
        };

        var files = await mainWindow.StorageProvider.OpenFilePickerAsync(options);
        return files.Count > 0 ? files[0].Path.LocalPath : null;
    }

    public async Task<string?> ShowSaveFileAsync(string title, string filter)
    {
        var mainWindow = GetMainWindow();
        if (mainWindow is null)
        {
            return null;
        }

        var options = new FilePickerSaveOptions
        {
            Title = title
        };

        var file = await mainWindow.StorageProvider.SaveFilePickerAsync(options);
        return file?.Path.LocalPath;
    }
}
