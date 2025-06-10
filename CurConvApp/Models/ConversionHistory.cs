using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurConvApp.Models
{
    public class ConversionHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FromCurrency { get; set; } = "";
        public string ToCurrency { get; set; } = "";
        public decimal AmountFrom { get; set; }
        public decimal AmountTo { get; set; }
        public DateTime ConversionDateTime { get; set; }

        // Навігаційна властивість (за бажанням)
        public DbUser User { get; set; }
    }

}
