using System.Linq;
using System.Windows;
using CurConvApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CurConvApp.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(int userId)
        {
            InitializeComponent();

            using (var db = new AppDbContext(
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseSqlServer(App.AppConfiguration.GetConnectionString("DefaultConnection")).Options))
            {
                var history = db.ConversionHistories
                    .Where(h => h.UserId == userId)
                    .OrderByDescending(h => h.ConversionDateTime)
                    .ToList();

                HistoryList.ItemsSource = history;
            }
        }
    }
}
