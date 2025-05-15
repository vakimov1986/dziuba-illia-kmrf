using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurConvApp.Models
{
    public class CurrencyRate
    {
        public string StartDate { get; set; } = string.Empty;
        public string CurrencyCodeL { get; set; } = string.Empty;

        public int Units { get; set; }
        public decimal Amount { get; set; }
    }
}

