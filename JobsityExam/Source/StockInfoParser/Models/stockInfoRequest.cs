using System.ComponentModel.DataAnnotations;

namespace StockInfoAPI.Models
{
    public class stockInfoRequest
    {
        [MinLength(1)]
        public string StockName { get; set; }
    }
}
