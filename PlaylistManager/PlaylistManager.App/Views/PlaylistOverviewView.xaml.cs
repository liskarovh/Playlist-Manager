using PlaylistManager.App.ViewModels;

namespace PlaylistManager.App.Views;

public partial class PlaylistOverviewView
{
	public PlaylistOverviewView(PlaylistOverviewViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
