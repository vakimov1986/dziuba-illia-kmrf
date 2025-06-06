﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurConvApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Windows;

namespace CurConvApp.ViewModels
{
    public partial class RegistrationViewModel : ObservableObject
    {
        [ObservableProperty] private string name = string.Empty;
        [ObservableProperty] private string surname = string.Empty;
        [ObservableProperty] private string email = string.Empty;
        [ObservableProperty] private string password = string.Empty;
        [ObservableProperty] private string message = string.Empty;

        
        //конструктор
        private readonly Action<string> _navigate;

        public RegistrationViewModel(Action<string> navigate)
        {
            _navigate = navigate;
        }


        [RelayCommand]
        private void GoToLogin()
        {
            _navigate("Login");
        }

        [RelayCommand]
        private void Register()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                Message = "Email і пароль обов’язкові.";
                return;
            }

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection"))
                .Options;

            using var db = new AppDbContext(options);
            var service = new Services.UserService(db);

            var success = service.RegisterUser(new DbUser
            {
                Name = Name,
                Surname = Surname,
                Email = Email,
                PasswordHash = Password
            });

            if (success)
            {
                Message = "Реєстрація успішна!";
                _navigate("Converter"); // ПЕРЕХІД до конвертера
            }
            else
            {
                Message = "Користувач з таким email вже існує.";
            }
        }
    }
}
