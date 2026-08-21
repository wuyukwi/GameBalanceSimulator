namespace GameBalanceSimulator.ViewModels.Services;

/// <summary>
/// Abstraction for showing platform-native dialogs from ViewModels.
/// </summary>
public interface IDialogService
{
    Task ShowInfoAsync(string message, string? title = null);

    Task<bool> ShowConfirmAsync(string message, string? title = null);

    Task<string?> ShowOpenFileAsync(string title, string filter);

    Task<string?> ShowSaveFileAsync(string title, string filter);
}
