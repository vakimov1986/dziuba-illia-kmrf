using System;
using System.Windows;
using CurConvApp.Views;
using CurConvApp.ViewModels;
using System.Windows.Threading;
using CurConvApp.Services.Authentification;
using CurConvApp.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CurConvApp
{
    public partial class MainWindow : Window
    {
        IUserRepository _userRepository;
        IAuthService _authService;

        public MainWindow()
        {
           InitializeComponent();

            //use ApplicationViewModel
            //cretate this code in App.xaml, override OnStartUp
            //Dependency injetion principle(SOLID(D))
            var options = new DbContextOptionsBuilder<AppDbContext>()
                 .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection"))
                 .Options;
            var db = new AppDbContext(options);
            _userRepository = new UserRepository(db);
            _authService = new AuthService(_userRepository, new BcryptAuthenticator());

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
            var vm = new RegistrationViewModel(_authService, NavigateTo);
            var view = new RegistrationView(NavigateTo)
            {
                DataContext = vm
            };
            
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
