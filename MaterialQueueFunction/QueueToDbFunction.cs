using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using MaterialQueueFunction.Model;
using MaterialQueueFunction.DAL;

namespace MaterialQueueFunction
{
    public class QueueToDbFunction
    {
        private readonly ILogger<QueueToDbFunction> _logger;
        private readonly IRepository _repository;

        public QueueToDbFunction(ILogger<QueueToDbFunction> logger, IRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [Function("ProcessServiceBusMessage")]
        public async Task RunAsync(
            [ServiceBusTrigger("material_queue", Connection = "ServiceBusConnection")] string message)
        {
            _logger.LogInformation("📩 Received message: {message}", message);

            try
            {
                var material = JsonSerializer.Deserialize<Material>(message);
                if (material == null)
                {
                    _logger.LogWarning("⚠️ Deserialized material is null. Skipping.");
                    return;
                }

                await _repository.AddMaterialAsync(material);

                _logger.LogInformation("✅ Material processed and inserted into SQL Server.");
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "❌ JSON deserialization failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to process message.");
                throw;
            }
        }
    }
}
