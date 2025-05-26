using PlaylistManager.App.ViewModels;

namespace PlaylistManager.App.Views;

public partial class PlaylistSelectedView : ContentPageBase
{
	public PlaylistSelectedView(PlaylistSelectedViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
