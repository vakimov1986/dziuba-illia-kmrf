using System.Windows;
using System.Windows.Controls;
using CurConvApp.ViewModels;

namespace CurConvApp.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView(Action<string> navigate)
        {
            InitializeComponent();
            DataContext = new LoginViewModel(navigate); // передаємо параметр
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LoginViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}
