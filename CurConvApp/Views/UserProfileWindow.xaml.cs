using System.Windows;
using CurConvApp.Models;
using CurConvApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CurConvApp.Views
{
    public partial class UserProfileWindow : Window
    {
        private DbUser _user;

        public UserProfileWindow(DbUser user)
        {
            InitializeComponent();
            _user = user;
            // Заповнюємо поля
            NameBox.Text = user.Name;
            SurnameBox.Text = user.Surname;
            EmailBox.Text = user.Email;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Перевірка пароля (за бажанням, якщо змінюєш пароль)
            string oldPass = OldPasswordBox.Password;
            string newPass = NewPasswordBox.Password;
            string repeatPass = RepeatPasswordBox.Password;

            // Якщо змінюємо пароль — перевіряємо старий і співпадіння нового
            if (!string.IsNullOrWhiteSpace(newPass) || !string.IsNullOrWhiteSpace(repeatPass))
            {
                if (newPass != repeatPass)
                {
                    MessageBox.Show("Новий пароль і повтор не співпадають.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(oldPass))
                {
                    MessageBox.Show("Введіть старий пароль.");
                    return;
                }
            }

            using (var db = new AppDbContext(
                new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection")).Options))
            {
                var dbUser = db.Users.FirstOrDefault(u => u.Id == _user.Id);
                if (dbUser != null)
                {
                    dbUser.Name = NameBox.Text;
                    dbUser.Surname = SurnameBox.Text;
                    dbUser.Email = EmailBox.Text;

                    // Зміна пароля
                    if (!string.IsNullOrWhiteSpace(newPass))
                    {
                        if (!PasswordHelper.VerifyPassword(oldPass, dbUser.PasswordHash))
                        {
                            MessageBox.Show("Старий пароль невірний!");
                            return;
                        }
                        dbUser.PasswordHash = PasswordHelper.HashPassword(newPass);
                    }



                    db.SaveChanges();
                    UserSessionManager.Instance.CurrentUser = dbUser;
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Користувача не знайдено в базі даних!");
                }
            }
        }


        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        
    }
}
