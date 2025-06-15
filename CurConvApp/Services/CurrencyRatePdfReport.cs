using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using System.Collections.Generic;
using System;
using CurConvApp.Models;

namespace CurConvApp.Reports
{
    public class CurrencyRatePdfReport : IDocument
    {
        private readonly string currency;
        private readonly List<CurrencyRateRecord> rates;
        private readonly DateTime startDate;
        private readonly DateTime endDate;


        public CurrencyRatePdfReport(string currency, List<CurrencyRateRecord> rates, DateTime startDate, DateTime endDate)
        {
            this.currency = currency;
            this.rates = rates;
            this.startDate = startDate;
            this.endDate = endDate;

        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(30);

                    page.Header()
                        .Text($"Звіт по курсу {currency} за період {startDate:dd.MM.yyyy} — {endDate:dd.MM.yyyy}")
                        .FontSize(18)
                        .Bold()
                        .AlignCenter();

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1); // Дата
                                columns.RelativeColumn(1); // Курс
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Дата").Bold();
                                header.Cell().Text("Курс").Bold();
                            });

                            foreach (var rate in rates)
                            {
                                table.Cell().Text(rate.StartDate.ToShortDateString());
                                table.Cell().Text(rate.Amount.ToString("F4"));
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Згенеровано: {DateTime.Now:dd.MM.yyyy HH:mm}");
                });
        }

        //pdf з малюнком
        public static void ExportToPdfWithImage(string filePath, string chartImagePath, List<CurrencyRateRecord> history)
        {
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);

                    page.Content().Column(col =>
                    {
                        col.Item().Text("Історія курсу валют").FontSize(24).SemiBold();

                        // Додаємо зображення графіка
                        col.Item().Image(chartImagePath, ImageScaling.FitWidth);

                        // Додаємо таблицю з історією (приклад)
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(120);
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text("Дата").SemiBold();
                                header.Cell().Text("Курс").SemiBold();
                            });

                            foreach (var rec in history)
                            {
                                table.Cell().Text($"{rec.StartDate:dd.MM.yyyy}");
                                table.Cell().Text($"{rec.Amount}");
                            }
                        });
                    });
                });
            })
            .GeneratePdf(filePath);
        }

    }
}
