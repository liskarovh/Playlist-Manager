using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace PlaylistManager.App.Converters;

[AcceptEmptyServiceProvider]
public partial class InvertBoolConverter : BaseConverterOneWay<bool, bool>
{
    public override bool ConvertFrom(bool value, CultureInfo? culture)
    {
        return !value;
    }

    public override bool DefaultConvertReturnValue { get; set; } = false;
}
