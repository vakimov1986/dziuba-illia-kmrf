using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CurConvApp.Services;
using CurConvApp.ViewModels;


namespace CurConvApp.Views
{
    public partial class CurrencyConverterView : UserControl
    {
        public CurrencyConverterView()
        {
            InitializeComponent();

            SubscribeToViewModelEvents();

            this.DataContextChanged += (s, e) =>
            {
                SubscribeToViewModelEvents();
            };
        }

        private void SubscribeToViewModelEvents()
        {
            if (DataContext is CurrencyConverterViewModel vm)
            {
                vm.SnackbarRequested -= ShowSnackbarPublic; // Щоб не підписувати двічі
                vm.SnackbarRequested += ShowSnackbarPublic;

                vm.ShowChartRequested -= OpenChartWindow;
                vm.ShowChartRequested += OpenChartWindow;
            }
        }


        private void OpenChartWindow()
        {
            var win = new CurrencyRateChartWindow();
            win.ShowDialog();
        }
        public void ShowSnackbarPublic(string message) => ShowSnackbar(message);
        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            var user = UserSessionManager.Instance.CurrentUser;
            if (user == null)
            {
                MessageBox.Show("Користувач не авторизований!");
                return;
            }
            var wnd = new UserProfileWindow(user) // Pass the required 'user' parameter to the constructor
            {
                NameBox = { Text = user.Name },
                EmailBox = { Text = user.Email }
            };
            if (wnd.ShowDialog() == true)
            {
                // Після збереження можна оновити відображення, якщо потрібно
                MessageBox.Show("Дані користувача оновлено!");
            }
        }


        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var currentTheme = Application.Current.Resources.MergedDictionaries.FirstOrDefault();
            if (currentTheme != null && currentTheme.Source != null && currentTheme.Source.OriginalString.Contains("DarkTheme.xaml"))
                Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
                {
                    Source = new Uri("Themes/LightTheme.xaml", UriKind.Relative)
                };
            else
                Application.Current.Resources.MergedDictionaries[0] = new ResourceDictionary()
                {
                    Source = new Uri("Themes/DarkTheme.xaml", UriKind.Relative)
                };
        }


        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
          "Автоматизований облік та конвертація валют у фінансових розрахунках\n\n" +
          "Програма дозволяє швидко отримувати курси валют, виконувати конвертацію, зберігати історію операцій та будувати графіки зміни курсів. Зручний інтерфейс, автоматичне оновлення даних і підтримка різних тем оформлення забезпечують комфортну роботу для бухгалтера, аналітика чи будь-якого користувача.\n\n" +
          "Розробники:\nстудент групи ПР-411 Ілля Дзюба\nкерівник проєкту Владислав Акімов",
          "Про програму"
        );

        }

        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            var currCulture = WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture;
            // Перемикаємо між англійською та українською
            if (currCulture.Name == "uk-UA")
                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("en");
            else
                WPFLocalizeExtension.Engine.LocalizeDictionary.Instance.Culture = new System.Globalization.CultureInfo("uk-UA");
        }
        

        //метод для показу pop-up
        private async void ShowSnackbar(string message)
        {
            SnackbarText.Text = message;
            Snackbar.Visibility = Visibility.Visible;

            // Fade in
            var fadeIn = new System.Windows.Media.Animation.DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(200));
            Snackbar.BeginAnimation(OpacityProperty, fadeIn);

            await Task.Delay(2500); // 2.5 секунди

            // Fade out
            var fadeOut = new System.Windows.Media.Animation.DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(500));
            Snackbar.BeginAnimation(OpacityProperty, fadeOut);

            await Task.Delay(600); // Після анімації
            Snackbar.Visibility = Visibility.Collapsed;
        }


        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            // Питання підтвердження (опціонально)
            if (MessageBox.Show("Вийти з програми?", "Підтвердження виходу",
                                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown(); // Коректне завершення роботи WPF-додатку
            }
        }

    }
}
