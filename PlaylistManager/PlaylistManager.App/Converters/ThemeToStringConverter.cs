using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace PlaylistManager.App.Converters;

[AcceptEmptyServiceProvider]
public partial class ThemeToStringConverter : BaseConverterOneWay<bool, string>
{
    public override string ConvertFrom(bool value, CultureInfo? culture)
    {
        return value ? "Switch to light theme" : "Switch to dark theme";
    }

    public override string DefaultConvertReturnValue { get; set; } = "RIP";
}
