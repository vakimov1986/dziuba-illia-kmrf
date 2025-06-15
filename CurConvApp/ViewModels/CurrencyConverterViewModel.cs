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

        public ObservableCollection<CurrencyRate> CurrencyRatesForListView { get; } = new();
        public ObservableCollection<CurrencyRateRecord> CurrencyRatesHistory { get; } = new();


        public CurrencyConverterViewModel()
        {
            _apiClient = new CurrencyApiClient();
            CurrencyRates = new ObservableCollection<CurrencyRate>();

            _autoLoadTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromHours(1) // оновлювати курси (щогодини)
            };
            _autoLoadTimer.Tick += async (s, e) => await LoadAndSaveRatesAsync("Автономне");
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
            _ = LoadAndSaveRatesAsync("Автономне (перший запуск)");
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

            // --- Додаємо збереження в історію конвертацій ---
            try
            {
                var user = UserSessionManager.Instance.CurrentUser;
                if (user != null)
                {
                    using (var db = new AppDbContext(
                        new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<AppDbContext>()
                        .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection")).Options))
                    {
                        var history = new ConversionHistory
                        {
                            UserId = user.Id,
                            FromCurrency = FromCurrency.CurrencyCodeL,
                            ToCurrency = ToCurrency.CurrencyCodeL,
                            AmountFrom = amount,
                            AmountTo = result,
                            ConversionDateTime = DateTime.Now // дата і час конвертації
                        };
                        db.ConversionHistories.Add(history);
                        db.SaveChanges();
                    }
                }
                // якщо користувач не авторизований — не зберігаємо історію
            }
            catch (Exception ex)
            {
                // Можна повідомити про помилку, або записати лог
                // MessageBox.Show($"Не вдалося зберегти історію: {ex.Message}");
            }
        }

        public event Action? ShowChartRequested;

        [RelayCommand]
        private void ShowChart()
        {
            ShowChartRequested?.Invoke();
        }

        [RelayCommand]
        private void ShowHistory()
        {
            var user = UserSessionManager.Instance.CurrentUser;
            if (user == null)
            {
                System.Windows.MessageBox.Show("Користувач не авторизований!");
                return;
            }
            // owner = null — якщо немає прямого доступу до вікна, або можна пробросити
            HistoryWindowService.ShowHistoryWindow(user.Id, null);
        }

        /// <summary>
        /// Єдиний DRY метод для обох режимів
        /// </summary>
        /// 

        [RelayCommand]
        private void ForecastRate()
        {
            if (ToCurrency == null)
            {
                StatusMessage = "Оберіть валюту для прогнозу.";
                return;
            }
            LoadCurrencyHistory(ToCurrency.CurrencyCodeL);

            if (CurrencyRatesHistory.Count < 2)
            {
                StatusMessage = "Недостатньо даних для прогнозу.";
                return;
            }

            var recentRates = CurrencyRatesHistory
                .OrderBy(r => r.StartDate)
                .Select(r => r.Amount)
                .ToList();

            decimal sumDelta = 0;
            for (int i = 1; i < recentRates.Count; i++)
                sumDelta += (recentRates[i] - recentRates[i - 1]);

            decimal avgDelta = sumDelta / (recentRates.Count - 1);
            decimal forecast = recentRates.Last() + avgDelta;

            StatusMessage = $"Прогноз курсу {ToCurrency.CurrencyCodeL} на завтра: {forecast:F4}";
            //
           // StatusMessage = $"Прогноз курсу ...";
            ShowSnackbar(StatusMessage);
        }

        private async Task LoadAndSaveRatesAsync(string source)
        {
            try
            {
                var rates = await _apiClient.GetRatesAsync();
                var filtered = rates
                    .Where(r => AllowedCurrencies.Contains(r.CurrencyCodeL))
                    .OrderBy(r => r.CurrencyCodeL);

                // Додаємо гривню
                var rateList = filtered.ToList();
                if (!rateList.Any(r => r.CurrencyCodeL == "UAH"))
                {
                    rateList.Insert(0, new CurrencyRate
                    {
                        CurrencyCodeL = "UAH",
                        Units = 1,
                        Amount = 1,
                        // StartDate = DateTime.Now.ToString("yyyy-MM-dd")
                    });
                }

                CurrencyRates.Clear();
                foreach (var rate in rateList)
                    CurrencyRates.Add(rate);

                // ОНОВЛЕННЯ CurrencyRatesForListView (без UAH)
                CurrencyRatesForListView.Clear();
                foreach (var rate in rateList.Where(r => r.CurrencyCodeL != "UAH"))
                    CurrencyRatesForListView.Add(rate);
                //UAH

                //CurrencyRates.Clear();
                //foreach (var rate in filtered)
                //    CurrencyRates.Add(rate);

                var dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection"))
                    .Options);

                var rateService = new CurrencyRateService(dbContext);
                rateService.SaveRates(filtered);

                StatusMessage = $"{source} оновлено ({DateTime.Now:yyyy-MM-dd HH:mm:ss})";
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

            // Додаємо UAH, якщо її нема
            var rateList = filtered.ToList();

            if (!rateList.Any(r => r.CurrencyCodeL == "UAH"))
            {
                rateList.Insert(0, new CurrencyRate
                {
                    CurrencyCodeL = "UAH",
                    Units = 1,
                    Amount = 1,
                    //  StartDate = DateTime.Now.ToString("yyyy-MM-dd")
                });
            }

            CurrencyRates.Clear();
            foreach (var rate in rateList)
                CurrencyRates.Add(rate);

            // ОНОВЛЕННЯ CurrencyRatesForListView (без UAH)
            CurrencyRatesForListView.Clear();
            foreach (var rate in rateList.Where(r => r.CurrencyCodeL != "UAH"))
                CurrencyRatesForListView.Add(rate);
            //  UAH

            //CurrencyRates.Clear();
            //foreach (var rate in filtered)
            //    CurrencyRates.Add(rate);

            var dbContext = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection"))
                .Options);

            var rateService = new CurrencyRateService(dbContext);
            rateService.SaveRates(filtered);

            StatusMessage = $"Оновлено ({DateTime.Now:yyyy-MM-dd HH:mm:ss})";

        }

        private void LoadCurrencyHistory(string currencyCode)
        {
            CurrencyRatesHistory.Clear();

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection"))
                .Options;

            using (var db = new AppDbContext(options))
            {
                var history = db.CurrencyRateRecords
                    .Where(r => r.CurrencyCodeL == currencyCode)
                    .OrderByDescending(r => r.StartDate)
                    .Take(7)
                    .ToList();

                foreach (var rate in history)
                    CurrencyRatesHistory.Add(rate);
            }
        }
        
        //pop-up
        public event Action<string>? SnackbarRequested;

        //метод для виклику pop-up
        private void ShowSnackbar(string message)
        {
            SnackbarRequested?.Invoke(message);
        }

        
    }
}
