using System.Windows;
using System.Windows.Controls;
using CurConvApp.ViewModels;

namespace CurConvApp.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView(Action<string> navigate)
        {
          //  MessageBox.Show("LoginView constructor called");
            InitializeComponent();
           // DataContext = new LoginViewModel(navigate);
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
