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

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            var currCulture = WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture;
            if (currCulture.Name == "uk-UA")
                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("en");
            else
                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("uk-UA");
        }

    }
}
