using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurConvApp.Models;

namespace CurConvApp.Services
{
    public class CurrencyApiClient
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<List<CurrencyRate>> GetRatesAsync()
        {
            string url = "https://bank.gov.ua/NBU_Exchange/exchange?json";
            var response = await _httpClient.GetStringAsync(url);

            var data = JsonSerializer.Deserialize<List<CurrencyRate>>(response);
            return data ?? new List<CurrencyRate>();
        }

    }
}
