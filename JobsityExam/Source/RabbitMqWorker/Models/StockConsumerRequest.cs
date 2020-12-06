using System.ComponentModel.DataAnnotations;

namespace RabbitMqWorker.Models
{
    public class StockConsumerRequest
    {
        [MinLength(1)]
        public string StockName { get; set; }
    }
}
