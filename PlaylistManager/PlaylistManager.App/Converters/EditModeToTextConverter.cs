using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace PlaylistManager.App.Converters;

[AcceptEmptyServiceProvider]
public partial class EditModeToTextConverter : BaseConverterOneWay<bool, string>
{
    public override string ConvertFrom(bool value, CultureInfo? culture)
    {
        return value ? "     Switch to\nbrowsing mode" : "   Switch to\nediting mode";
    }

    public override string DefaultConvertReturnValue { get; set; } = "";
}
