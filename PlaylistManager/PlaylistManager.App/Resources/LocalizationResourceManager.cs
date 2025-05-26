using System.Globalization;
using PlaylistManager.App.Resources.Texts;
using PropertyChanged;

namespace PlaylistManager.App.Resources;

[AddINotifyPropertyChangedInterface]
public partial class LocalizationResourceManager
{
    public static LocalizationResourceManager Instance { get; } = new();

    public CultureInfo CurrentCulture { get; set; } = CultureInfo.CurrentUICulture;

    // Dictionary for storing key-value pairs
    public Dictionary<string, string> LocalizedValues { get; set; } = new();

    public string this[string key]
    {
        get
        {
            if (!LocalizedValues.TryGetValue(key, out var value))
            {
                value = AppTexts.ResourceManager.GetString(key, CurrentCulture) ?? $"[{key}]";
                LocalizedValues[key] = value;
            }
            return value;
        }
    }

    public void SwitchCulture(CultureInfo newCulture)
    {
        if (CurrentCulture.Name != newCulture.Name)
        {
            CurrentCulture = newCulture;
            CultureInfo.CurrentUICulture = newCulture;
            CultureInfo.CurrentCulture = newCulture;

            ReloadAllKeys();
        }
    }

    public void ReloadAllKeys()
    {
        var keys = LocalizedValues.Keys.ToList();
        LocalizedValues.Clear();
        foreach (var key in keys)
        {
            LocalizedValues[key] = AppTexts.ResourceManager.GetString(key, CurrentCulture) ?? $"[{key}]";

            // Fody will raise PropertyChanged for this[key]
            this.OnPropertyChanged($"Item[{key}]");
        }
    }
}
