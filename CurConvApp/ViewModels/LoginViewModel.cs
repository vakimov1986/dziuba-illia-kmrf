using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurConvApp.Models;
using CurConvApp.Services;
using CurConvApp.Services.Authentification;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Windows;

namespace CurConvApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty] 
        private string email = string.Empty;

        [ObservableProperty] 
        private string password = string.Empty;

        [ObservableProperty] 
        private string message = string.Empty;

       // Конструктор
        private readonly Action<string> _navigate;
        private readonly IAuthService authService;

        public LoginViewModel(Action<string> navigate)
        {
            _navigate = navigate;
        }

        public LoginViewModel(IAuthService authService)
        {
            this.authService = authService;
        }

        [RelayCommand]
        private void GoToRegister()
        {
            _navigate("Register");
        }

        [RelayCommand]
        private void Login()
        {
            // NOTE : Violation of SRP(SOLID principles)

            //validate user
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Message = "Уведіть email і пароль.";
                return;
            }


            // NOTE : it is better to use Dependency injection (SOLID principles, letter D) IAuthService

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection"))
                .Options;


            using var db = new AppDbContext(options);
            var service = new Services.UserService(db);

            var user = service.AuthenticateUser(Email, Password);
            if (user != null)
            {
                Message = "Вхід успішний.";
                _navigate("Converter"); // ПЕРЕХІД до конвертера
            }
            else
            {
                Message = "Невірний email або пароль.";
            }

            //var result = authService.Authenticate(email, password);
            
            //if (result.IsFailed) 
            //{
            //    Message = result.Error;
            //    return;
            //}

            //Message = "Вхід успішний.";
            //_navigate("Converter"); // ПЕРЕХІД до конвертера
        }
    }
}
