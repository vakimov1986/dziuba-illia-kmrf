using System.Windows.Controls;
using CurConvApp.ViewModels;

namespace CurConvApp.Views
{
    public partial class CurrencyRateChartView : UserControl
    {
        public CurrencyRateChartView()
        {
            InitializeComponent();
            DataContext = new CurrencyRateChartViewModel();
        }
    }
}
