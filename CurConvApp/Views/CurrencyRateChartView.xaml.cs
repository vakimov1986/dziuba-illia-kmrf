using System.Windows.Controls;
using CurConvApp.ViewModels;

namespace CurConvApp.Views
{
    public partial class CurrencyRateChartView : UserControl
    {
        public CurrencyRateChartView(string currencyCode, DateTime start, DateTime end)
        {
            InitializeComponent();
            DataContext = new CurrencyRateChartViewModel
            {
                CurrencyCode = currencyCode,
                StartDate = start,
                EndDate = end
            };
        }
    }
}
