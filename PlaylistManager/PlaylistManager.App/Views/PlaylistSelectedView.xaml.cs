using PlaylistManager.App.ViewModels;

namespace PlaylistManager.App.Views;

public partial class PlaylistSelected : ContentPageBase
{
	public PlaylistSelected(PlaylistSelectedViewModel viewModel) : base(viewModel)
	{
		InitializeComponent();
	}
}
