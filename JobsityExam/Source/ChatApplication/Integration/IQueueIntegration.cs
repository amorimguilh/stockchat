using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ChatApplication.Integration
{
    public interface IQueueIntegration
    {
        Task PublishMessage(string message);
    }
}
