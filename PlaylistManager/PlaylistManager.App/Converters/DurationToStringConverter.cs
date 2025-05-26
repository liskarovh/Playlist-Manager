using CommunityToolkit.Maui.Converters;
using System.Globalization;

namespace PlaylistManager.App.Converters;

[AcceptEmptyServiceProvider]
public partial class DurationToStringConverter : BaseConverterOneWay<double, string>
{
    public override string ConvertFrom(double value, CultureInfo? culture)
    {
        uint intVal = (uint)value;

        return intVal switch
        {
            < 60 => $"{intVal}s",
            < 3600 => $"{intVal / 60}m {intVal % 60}s",
            _ => $"{intVal / 3600}h {(intVal / 60) % 60}m {intVal % 60}s"
        };
    }

    public override string DefaultConvertReturnValue { get; set; } = "0s";
}
