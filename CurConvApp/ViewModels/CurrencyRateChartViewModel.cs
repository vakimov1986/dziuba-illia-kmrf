using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CurConvApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace CurConvApp.ViewModels
{
    public partial class CurrencyRateChartViewModel : ObservableObject
    {
        [ObservableProperty]
        private PlotModel chartModel;

        // Обирана валюта та діапазон дат
        [ObservableProperty] private string currencyCode = "USD";
        [ObservableProperty] private DateTime startDate = DateTime.Now.AddMonths(-1);
        [ObservableProperty] private DateTime endDate = DateTime.Now;

        public CurrencyRateChartViewModel()
        {
            // Автоматично будуємо графік при створенні ViewModel (або викликай LoadChartAsync вручну)
            Task.Run(async () => await LoadChartAsync());
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

        private PlotModel BuildChartModel(List<CurrencyRateRecord> rates, string currencyCode)
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
    }
}
