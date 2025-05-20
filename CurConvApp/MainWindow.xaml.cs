using System;
using System.Windows;
using CurConvApp.Views;
using CurConvApp.ViewModels;

namespace CurConvApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowLogin();
        }

        private void ShowLogin()
        {
            var view = new LoginView(NavigateTo);
            var vm = new LoginViewModel(NavigateTo);
            view.DataContext = vm;
            MainContent.Content = view;
        }

        private void ShowRegister()
        {
            var view = new RegistrationView(NavigateTo);
            var vm = new RegistrationViewModel(NavigateTo);
            view.DataContext = vm;
            MainContent.Content = view;
        }

        private void ShowCurrencyConverter()
        {
            var view = new CurrencyConverterView();
            view.DataContext = new CurrencyConverterViewModel();
            MainContent.Content = view;
        }

        private void NavigateTo(string target)
        {
            switch (target)
            {
                case "Login": ShowLogin(); break;
                case "Register": ShowRegister(); break;
                case "Converter": ShowCurrencyConverter(); break;
            }
        }
    }
}
