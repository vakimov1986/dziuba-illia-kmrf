﻿using System.ComponentModel.DataAnnotations;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurConvApp.Models;
using CurConvApp.Services;
using CurConvApp.Services.Authentification;
using CurConvApp.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CurConvApp.ViewModels
{
    public partial class RegistrationViewModel : ObservableValidator
    {
        private readonly Action<string> _navigate;
        private readonly IAuthService _authService;

        //конструктор
        public RegistrationViewModel(IAuthService authService, Action<string> navigate)
        {
            _navigate = navigate;
            _authService = authService;
        }

        //or we can use more simplier variant
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Name is required.")]
        private string name = string.Empty;
       
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Surname is required.")]
        private string surname = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Email is required.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email format.")]
        private string email = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        private string password = string.Empty;

        [ObservableProperty] 
        private string message = string.Empty;
        
        
        [RelayCommand]
        private void GoToLogin()
        {
            _navigate("Login");
        }

        [RelayCommand]
        private void Register()
        {

            ValidateAllProperties();

            ShowAllErrors();

            if (HasErrors)
            {
                return;
            }


            //if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            //{
            //    Message = "Email і пароль обов’язкові.";
            //    return;
            //}

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
                // Знайти доданого користувача у базі (щоб був з Id)
                var registeredUser = db.Users.FirstOrDefault(u => u.Email == Email);

                // Додати у сесію
                if (registeredUser != null)
                    UserSessionManager.Instance.CurrentUser = registeredUser;

                Message = "Реєстрація успішна!";
                _navigate("Converter"); // ПЕРЕХІД до конвертера
            }
            else
            {
                Message = "Користувач з таким email вже існує.";
            }

            //var user = new User()
            //{
            //    Name = name,
            //    Surname = surname,
            //    Email = email,
            //    Password = password
            //};
            //var result = _authService.Register(user);

            //if (result.IsFailed)
            //{
            //    Message = result.Error;
            //    return;
            //}

            //Message = "Вхід успішний.";
            //_navigate("Converter"); // ПЕРЕХІД до конвертера
        }

        public void ShowAllErrors()
        {
            var props = new[] { nameof(Name), nameof(Surname), nameof(Email), nameof(Password) };
            string errorText = "";

            foreach (var prop in props)
            {
                var errs = GetErrors(prop);
                if (errs != null)
                {
                    foreach (var err in errs)
                    {
                        errorText += $"Поле {prop}: {err}\n";
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(errorText))
                System.Windows.MessageBox.Show(errorText, "Помилки валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
        }


    }
}
