using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurConvApp.Services;

public partial class UserProfileViewModel : ObservableObject
{
    // Інші властивості/команди…

    [RelayCommand]
    private void ShowHistory()
    {
        var user = UserSessionManager.Instance.CurrentUser;
        if (user == null)
        {
            System.Windows.MessageBox.Show("Користувач не авторизований!");
            return;
        }
        HistoryWindowService.ShowHistoryWindow(user.Id, null);
    }
}
