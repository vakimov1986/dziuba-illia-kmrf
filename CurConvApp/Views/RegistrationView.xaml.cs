using System.Windows;
using System.Windows.Controls;
using CurConvApp.ViewModels;


namespace CurConvApp.Views
{
    /// <summary>
    /// Interaction logic for RegistrationView.xaml
    /// </summary>
    public partial class RegistrationView : UserControl
    {
        public RegistrationView(Action<string> navigate)
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is RegistrationViewModel vm)
            {
                vm.Password = ((PasswordBox)sender).Password;
            }
        }


    }
}
