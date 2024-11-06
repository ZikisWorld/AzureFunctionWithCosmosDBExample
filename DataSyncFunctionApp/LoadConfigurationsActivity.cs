using DataSyncFunctionApp.Models;
using DataSyncFunctionApp.Services;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DataSyncFunctionApp
{
    public class LoadConfigurationsActivity
    {
        private readonly IConfiguration _configuration;

        public LoadConfigurationsActivity(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("LoadConfigurationsActivity")]
        public async Task<List<MappingModel>> Run([ActivityTrigger] object input, ILogger log)
        {
            // Retrieve the path from configuration
            var configFilePath = _configuration["MappingsConfigFile"];

            log.LogInformation($"Loading configurations from {configFilePath}");

            // Load configurations using the ConfigLoader
            var configurations = MappingService.LoadTableConfigurations(configFilePath);

            log.LogInformation($"Loaded {configurations.Count} configurations from JSON.");
            return configurations;
        }
    }
}
