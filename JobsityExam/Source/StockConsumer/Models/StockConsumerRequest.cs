using System.ComponentModel.DataAnnotations;

namespace StockConsumer.Models
{
    public class StockConsumerRequest
    {
        [MinLength(1)]
        public string StockName { get; set; }
    }
}
