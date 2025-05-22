using System;
using System.Windows;
using CurConvApp.Views;
using CurConvApp.ViewModels;
using System.Windows.Threading;

namespace CurConvApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
           InitializeComponent();
           ShowSplashOverlay();
           ShowLogin();
        }
        private void ShowSplashOverlay()
        {
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3); // тривалість splash
            timer.Tick += (s, a) =>
            {
                timer.Stop();
                SplashOverlay.Visibility = Visibility.Collapsed;
            };
            timer.Start();
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

        public void NavigateTo(string target)
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
