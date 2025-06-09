using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CurConvApp.Models;
using CurConvApp.Reports;
using CurConvApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CurConvApp.ViewModels
{
    public partial class CurrencyRateChartViewModel : ObservableObject
    {
        public ObservableCollection<string> AllowedCurrencies { get; } = new()
        {
            "USD", "EUR", "GBP", "CHF", "CZK", "PLN", "HUF", "SEK", "RON", "JPY"
        };

        [ObservableProperty]
        private string currencyCode = "USD";
        partial void OnCurrencyCodeChanged(string value)
        {
            _ = LoadChartAsync();
        }

        [ObservableProperty]
        private DateTime startDate = new DateTime(2025, 5, 14);
        partial void OnStartDateChanged(DateTime value)
        {
            _ = LoadChartAsync();
        }

        [ObservableProperty]
        private DateTime endDate = DateTime.Now.AddDays(1);
        partial void OnEndDateChanged(DateTime value)
        {
            _ = LoadChartAsync();
        }

        [ObservableProperty]
        private PlotModel chartModel;

        public CurrencyRateChartViewModel()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            _ = LoadChartAsync();
        }

        public async Task LoadChartAsync()
        {
            var connectionString = App.AppConfiguration.GetConnectionString("DefaultConnection");
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new AppDbContext(options);

            var rates = await db.CurrencyRateRecords
                .Where(r => r.CurrencyCodeL == CurrencyCode && r.StartDate >= StartDate && r.StartDate <= EndDate)
                .OrderBy(r => r.StartDate)
                .ToListAsync();

            ChartModel = BuildChartModel(rates, CurrencyCode);
        }

        private PlotModel BuildChartModel(System.Collections.Generic.List<CurrencyRateRecord> rates, string currencyCode)
        {
            var model = new PlotModel { Title = $"Динаміка курсу {currencyCode}" };

            var lineSeries = new LineSeries
            {
                Title = currencyCode,
                MarkerType = MarkerType.Circle,
                MarkerSize = 4,
                MarkerStroke = OxyColors.SteelBlue
            };

            foreach (var rate in rates)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(rate.StartDate), (double)rate.Amount));
            }

            model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                StringFormat = "dd.MM",
                Title = "Дата"
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Курс"
            });

            model.Series.Add(lineSeries);

            return model;
        }

        [RelayCommand]
        private void ExportPdf()
        {
            ExportToPdf(CurrencyCode, StartDate, EndDate);
        }
        public void ExportToPdf(string currency, DateTime start, DateTime end)
        {
            var connectionString = App.AppConfiguration.GetConnectionString("DefaultConnection");
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            using var db = new AppDbContext(options);

            var rates = db.CurrencyRateRecords
                .Where(r => r.CurrencyCodeL == currency && r.StartDate >= start && r.StartDate <= end)
                .OrderBy(r => r.StartDate)
                .ToList();

            var report = new CurrencyRatePdfReport(currency, rates, start, end);

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"Звіт_{currency}_{DateTime.Now:yyyyMMdd}.pdf"
            };
            if (dialog.ShowDialog() == true)
            {
                report.GeneratePdf(dialog.FileName);
                System.Windows.MessageBox.Show("Експорт завершено!", "PDF", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }


    }
}
