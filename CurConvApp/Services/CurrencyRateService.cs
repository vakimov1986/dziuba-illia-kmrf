using CurConvApp.Models;

public class CurrencyRateService
{
    private readonly AppDbContext _context;

    public CurrencyRateService(AppDbContext context)
    {
        _context = context;
    }

    public void SaveRates(IEnumerable<CurrencyRate> rates)
    {
        foreach (var rate in rates)
        {
            // Перевірка дублікатів по даті та валюті
            bool exists = _context.CurrencyRateRecords
                .Any(r => r.CurrencyCodeL == rate.CurrencyCodeL && r.StartDate == DateTime.Parse(rate.StartDate));

            if (!exists)
            {
                var record = new CurrencyRateRecord
                {
                    CurrencyCodeL = rate.CurrencyCodeL,
                    //CurrencyCode = rate.CurrencyCode,
                    Units = rate.Units,
                    Amount = rate.Amount,
                    StartDate = DateTime.Parse(rate.StartDate)
                };

                _context.CurrencyRateRecords.Add(record);
            }
        }
        _context.SaveChanges();
    }
}
