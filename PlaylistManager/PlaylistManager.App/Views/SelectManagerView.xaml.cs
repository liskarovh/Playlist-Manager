using PlaylistManager.App.ViewModels;

namespace PlaylistManager.App.Views;

public partial class SelectManagerView : ContentPageBase
{
    public SelectManagerView(SelectManagerViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
