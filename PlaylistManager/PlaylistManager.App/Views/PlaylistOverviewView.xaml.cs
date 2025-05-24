using PlaylistManager.App.ViewModels;

namespace PlaylistManager.App.Views;

public partial class PlaylistOverview : ContentPageBase
{
	public PlaylistOverview(PlaylistOverviewViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
