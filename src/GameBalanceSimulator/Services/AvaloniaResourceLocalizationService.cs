using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using GameBalanceSimulator.ViewModels.Services;

namespace GameBalanceSimulator.Services;

public sealed class AvaloniaResourceLocalizationService : ILocalizationService
{
    private readonly Application _application;
    private readonly Dictionary<CultureInfo, ResourceDictionary> _cultureDictionaries;
    private readonly CultureInfo _fallbackCulture;

    private CultureInfo _currentCulture;

    public IReadOnlyList<CultureInfo> SupportedCultures { get; }

    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (_currentCulture.Name.Equals(value.Name, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _currentCulture = value;
            ApplyCulture(value);
            CultureChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler? CultureChanged;

    public AvaloniaResourceLocalizationService(Application application, IEnumerable<CultureInfo> supportedCultures)
    {
        _application = application;
        SupportedCultures = supportedCultures.ToList().AsReadOnly();

        if (SupportedCultures.Count == 0)
        {
            throw new ArgumentException("At least one supported culture must be provided.", nameof(supportedCultures));
        }

        _fallbackCulture = SupportedCultures[0];
        _currentCulture = _fallbackCulture;
        _cultureDictionaries = SupportedCultures.ToDictionary(
            culture => culture,
            LoadDictionary);
    }

    public void Initialize()
    {
        ApplyCulture(_currentCulture);
    }

    private static ResourceDictionary LoadDictionary(CultureInfo culture)
    {
        var uri = new Uri($"avares://GameBalanceSimulator/Assets/Strings/Strings.{culture.Name}.axaml");
        return (ResourceDictionary)AvaloniaXamlLoader.Load(uri);
    }

    private void ApplyCulture(CultureInfo culture)
    {
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        var merged = _application.Resources.MergedDictionaries;
        foreach (var dictionary in _cultureDictionaries.Values)
        {
            if (merged.Contains(dictionary))
            {
                merged.Remove(dictionary);
            }
        }

        merged.Add(_cultureDictionaries[culture]);
    }

    public string GetString(string key)
    {
        if (TryGetString(_application.Resources, key, out var value))
        {
            return value;
        }

        if (TryGetString(_cultureDictionaries[_fallbackCulture], key, out var fallback))
        {
            return fallback;
        }

        return key;
    }

    private static bool TryGetString(IResourceNode resources, string key, out string value)
    {
        if (resources.TryGetResource(key, null, out var raw) && raw is string text)
        {
            value = text;
            return true;
        }

        value = string.Empty;
        return false;
    }
}
