using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurConvApp.Models;
using CurConvApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CurConvApp.ViewModels
{
    public partial class CurrencyConverterViewModel : ObservableObject
    {
        private readonly CurrencyApiClient _apiClient;
        private readonly DispatcherTimer _autoLoadTimer;
        private readonly DispatcherTimer _uiTimer;
        private DateTime _nextUpdateTime;

        public CurrencyConverterViewModel()
        {
            _apiClient = new CurrencyApiClient();
            CurrencyRates = new ObservableCollection<CurrencyRate>();

            _autoLoadTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromHours(1) // оновлювати курси (щогодини)
            };
            _autoLoadTimer.Tick += async (s, e) => await LoadAndSaveRatesAsync("Автоматичне");
            _autoLoadTimer.Start();

            _nextUpdateTime = DateTime.Now.AddHours(1);
            _uiTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _uiTimer.Tick += (s, e) => UpdateTimeLeft();
            _uiTimer.Start();

            // Ініціалізація при старті
            _ = AutoLoadRatesAsync();

            // Перший запуск при старті програми
            _ = LoadAndSaveRatesAsync("Автоматичне (перший запуск)");
        }

        [ObservableProperty]
        private ObservableCollection<CurrencyRate> currencyRates;

        private static readonly HashSet<string> AllowedCurrencies = new()
        {
            "USD", "EUR", "GBP", "CHF", "CZK", "PLN", "HUF", "SEK", "RON", "JPY"
        };

        [ObservableProperty]
        private string inputAmount = "1";

        [ObservableProperty]
        private CurrencyRate? fromCurrency;

        [ObservableProperty]
        private CurrencyRate? toCurrency;

        [ObservableProperty]
        private string conversionResult = string.Empty;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private string timeLeft = string.Empty;

        /// <summary>
        /// Ручне завантаження (по кнопці)
        /// </summary>
        [RelayCommand]
        public async Task LoadRatesAsync()
        {
            await LoadAndSaveRatesAsync("Ручне");
        }

        [RelayCommand]
        private void Convert()
        {
            if (FromCurrency == null || ToCurrency == null)
            {
                ConversionResult = "Оберіть обидві валюти.";
                return;
            }

            if (!decimal.TryParse(InputAmount, out decimal amount))
            {
                ConversionResult = "Некоректна сума.";
                return;
            }

            decimal amountInUah = amount * (FromCurrency.Amount / FromCurrency.Units);
            decimal result = amountInUah / (ToCurrency.Amount / ToCurrency.Units);

            ConversionResult = $"{amount} {FromCurrency.CurrencyCodeL} = {result:F2} {ToCurrency.CurrencyCodeL}";
        }


        [RelayCommand]
        private void ShowChart()
        {
            // Дати, які треба вибрати (наприклад, останній місяць)
            var currency = FromCurrency?.CurrencyCodeL ?? "USD";
            var start = DateTime.Now.AddMonths(-1);
            var end = DateTime.Now;

            var chartView = new Views.CurrencyRateChartView(currency, start, end);
            var window = new Window
            {
                Title = $"Динаміка курсу {currency}",
                Content = chartView,
                Width = 700,
                Height = 400
            };
            window.ShowDialog();
        }


        /// <summary>
        /// Єдиний DRY метод для обох режимів
        /// </summary>
        private async Task LoadAndSaveRatesAsync(string source)
        {
            try
            {
                var rates = await _apiClient.GetRatesAsync();
                var filtered = rates
                    .Where(r => AllowedCurrencies.Contains(r.CurrencyCodeL))
                    .OrderBy(r => r.CurrencyCodeL);

                CurrencyRates.Clear();
                foreach (var rate in filtered)
                    CurrencyRates.Add(rate);

                var dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection"))
                    .Options);

                var rateService = new CurrencyRateService(dbContext);
                rateService.SaveRates(filtered);

                StatusMessage = $"{source} оновлення курсів та збереження ({DateTime.Now:HH:mm:ss})";
                _nextUpdateTime = DateTime.Now.AddHours(1);
                UpdateTimeLeft();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Помилка при {source.ToLower()} оновленні: {ex.Message}";
            }
        }

        //public async Task LoadRatesAsync()
        //{
        //    var rates = await _apiClient.GetRatesAsync();

        //    var filtered = rates
        //        .Where(r => AllowedCurrencies.Contains(r.CurrencyCodeL))
        //        .OrderBy(r => r.CurrencyCodeL);

        //    CurrencyRates.Clear();
        //    foreach (var rate in filtered)
        //        CurrencyRates.Add(rate);
        //}





        //   private readonly DispatcherTimer _autoLoadTimer; //поле таймера

        private void UpdateTimeLeft()
        {
            var time = _nextUpdateTime - DateTime.Now;
            if (time.TotalSeconds <= 0)
            {
                TimeLeft = "Оновлення найближчим часом...";
            }
            else
            {
                TimeLeft = $"Наступне оновлення через: {time:hh\\:mm\\:ss}";
            }
        }

        private async Task AutoLoadRatesAsync()
        {
            var rates = await _apiClient.GetRatesAsync();
            var filtered = rates
                .Where(r => AllowedCurrencies.Contains(r.CurrencyCodeL))
                .OrderBy(r => r.CurrencyCodeL);

            CurrencyRates.Clear();
            foreach (var rate in filtered)
                CurrencyRates.Add(rate);

            var dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection"))
                .Options);

            var rateService = new CurrencyRateService(dbContext);
            rateService.SaveRates(filtered);

            StatusMessage = $"Курси оновлено та збережено ({DateTime.Now:HH:mm:ss})";
        }



    }
}
