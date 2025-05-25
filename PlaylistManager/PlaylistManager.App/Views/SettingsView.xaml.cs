using PlaylistManager.App.ViewModels;

namespace PlaylistManager.App.Views;

public partial class SettingsView : ContentPageBase
{
	public SettingsView(SettingsViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
