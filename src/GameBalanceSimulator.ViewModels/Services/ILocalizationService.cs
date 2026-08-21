using System.Globalization;

namespace GameBalanceSimulator.ViewModels.Services;

/// <summary>
/// Abstraction for runtime localization.
/// </summary>
public interface ILocalizationService
{
    IReadOnlyList<CultureInfo> SupportedCultures { get; }

    CultureInfo CurrentCulture { get; set; }

    event EventHandler? CultureChanged;

    string GetString(string key);
}
