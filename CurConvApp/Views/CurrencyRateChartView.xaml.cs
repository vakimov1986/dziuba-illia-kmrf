using System.IO;
using System.Windows;
using System.Windows.Controls;
using CurConvApp.ViewModels;
using Microsoft.Win32;
using OxyPlot.Wpf;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace CurConvApp.Views
{
    public partial class CurrencyRateChartView : UserControl
    {
        public CurrencyRateChartView()
        {
            InitializeComponent();
            //  DataContext = new CurrencyRateChartViewModel();
            DataContext = new CurrencyRateChartViewModel(new FileDialogService());

            //if (DataContext is CurrencyRateChartViewModel vm)
            //    vm.ExportPdfRequested += ExportToPdf;
            //this.DataContextChanged += (s, e) =>
            //{
            //    if (e.NewValue is CurrencyRateChartViewModel vm2)
            //        vm2.ExportPdfRequested += ExportToPdf;
            //};


        }

        private void ExportToPdf()
        {
            // 1. Експортуємо графік у PNG
            string chartFile = "chart.png";
            var exporter = new PngExporter { Width = 800, Height = 500};
            using (var stream = File.Create(chartFile))
            {
                exporter.Export(this.PlotView.Model, stream);
                // PlotView — x:Name твого OxyPlot PlotView!
            }

            // 2. Створюємо PDF
            var pdf = new PdfDocument();
            var page = pdf.AddPage();
            page.Width = 850;
            page.Height = 600;
            var gfx = XGraphics.FromPdfPage(page);

            // Додаємо зображення графіка
            using (var img = XImage.FromFile(chartFile))
            {
                gfx.DrawImage(img, 30, 30, 400, 250);
            }

            // Додаємо текст таблиці курсів
            gfx.DrawString("Історія курсу:", new XFont("Arial", 16), XBrushes.Black, new XPoint(30, 300));
            double y = 330;

            var vm = this.DataContext as CurrencyRateChartViewModel;
            if (vm != null)
            {
                foreach (var rate in vm.CurrencyRatesHistory)
                {
                    gfx.DrawString($"{rate.StartDate:dd.MM.yyyy} : {rate.Amount}", new XFont("Arial", 12), XBrushes.Black, new XPoint(40, y));
                    y += 20;
                }
            }

            // 3. Зберігаємо PDF
            var dialog = new SaveFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf",
                FileName = $"CurrencyChart_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };
            if (dialog.ShowDialog() == true)
            {
                using (var file = File.Create(dialog.FileName))
                    pdf.Save(file);

                MessageBox.Show($"Файл PDF збережено як {dialog.FileName}!");
            }

        }


    }
}
