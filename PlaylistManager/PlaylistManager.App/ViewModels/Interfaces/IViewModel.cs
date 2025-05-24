namespace PlaylistManager.App.ViewModels
{
    public interface IViewModel
    {
        void LoadInDesignMode();
        Task OnAppearingAsync();
    }
}
