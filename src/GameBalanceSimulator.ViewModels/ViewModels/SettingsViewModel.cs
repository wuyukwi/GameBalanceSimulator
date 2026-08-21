using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using GameBalanceSimulator.ViewModels.Services;

namespace GameBalanceSimulator.ViewModels.ViewModels;

public sealed partial class SettingsViewModel : ViewModelBase
{
    private readonly ILocalizationService _localizationService;

    [ObservableProperty]
    private CultureInfo _selectedCulture;

    public IReadOnlyList<CultureInfo> SupportedCultures => _localizationService.SupportedCultures;

    public SettingsViewModel(ILocalizationService localizationService)
    {
        _localizationService = localizationService;
        _selectedCulture = localizationService.CurrentCulture;
    }

    partial void OnSelectedCultureChanged(CultureInfo value)
    {
        _localizationService.CurrentCulture = value;
    }
}
