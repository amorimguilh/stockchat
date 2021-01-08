using System.ComponentModel.DataAnnotations;

namespace RabbitMqWorker.Models
{
    /// <summary>
    /// Stock consumer web api payload
    /// </summary>
    public class StockConsumerRequest
    {
        /// <summary>
        /// Stock code
        /// </summary>
        [MinLength(1)]
        public string StockName { get; set; }
    }
}
