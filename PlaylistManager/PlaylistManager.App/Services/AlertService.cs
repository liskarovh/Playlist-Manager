namespace PlaylistManager.App.Services;

public class AlertService : IAlertService
{
    public async Task DisplayAsync(string title, string message)
    {
        var displayAlert = Application.Current?.Windows[0]?.Page?.DisplayAlert(title, message, "OK");

        if (displayAlert is not null)
        {
            await displayAlert;
        }
    }
}
