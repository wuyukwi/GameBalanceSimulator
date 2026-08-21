using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using GameBalanceSimulator.ViewModels.Services;
using GameBalanceSimulator.Views;

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

    public async Task ShowInfoAsync(string message, string? title = null)
    {
        var mainWindow = GetMainWindow();
        if (mainWindow is null)
        {
            return;
        }

        await MessageBoxWindow.ShowInfoAsync(mainWindow, message, title ?? string.Empty);
    }

    public async Task<bool> ShowConfirmAsync(string message, string? title = null)
    {
        var mainWindow = GetMainWindow();
        if (mainWindow is null)
        {
            return false;
        }

        return await MessageBoxWindow.ShowConfirmAsync(mainWindow, message, title ?? string.Empty);
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
