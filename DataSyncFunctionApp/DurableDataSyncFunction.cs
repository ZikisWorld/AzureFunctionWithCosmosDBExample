using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.Core.Configuration;
using DataSyncFunctionApp.DataAccess;
using DataSyncFunctionApp.Models;
using DataSyncFunctionApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.Marshalling.IIUnknownCacheStrategy;

namespace DataSyncFunctionApp
{
    public class DurableDataSyncFunction
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly ICosmosDataAccess _cosmosDataAccess;

        public DurableDataSyncFunction(ISqlDataAccess sqlDataAccess, ICosmosDataAccess cosmosDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
            _cosmosDataAccess = cosmosDataAccess;
        }

        [FunctionName("DurableDataSyncStarter")]
        public async Task RunStarter(
        [TimerTrigger("%DataSyncSchedule%")] TimerInfo myTimer,  // Adjust for daily run
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            log.LogInformation($"Durable Data Sync Starter function executed at: {DateTime.Now}");

            // Start the orchestration
            await starter.StartNewAsync("DurableDataSyncOrchestrator", null);
        }

        [FunctionName("DurableDataSyncOrchestrator")]
        public async Task RunOrchestrator(
    [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // Define the tables and columns for data sync in a sequential order
            //        var tables = new[]
            //        {
            //    new MappingModel { TableName = "Countries", ColumnName = "CountryName", FieldName = "CountriesDoc" },
            //    new MappingModel { TableName = "Cars", ColumnName = "CarModel", FieldName = "CarsDoc" }
            //};

            //// Load configurations
            //string configFilePath = "TableConfig.json";
            //var mappingModels = await context.CallActivityAsync<List<MappingModel>>(
            //    "LoadConfigurationsActivity", configFilePath);

            // Load configurations            
            var mappingModels = await context.CallActivityAsync<List<MappingModel>>(
                "LoadConfigurationsActivity", null);

            foreach (var model in mappingModels)
            {
                // Call the activity function sequentially for each table
                await context.CallActivityAsync("DurableDataSyncActivity", model);
            }
        }


        [FunctionName("DurableDataSyncActivity")]
        public async Task RunActivity(
    [ActivityTrigger] MappingModel mappingModel,
    ILogger log)
        {
            log.LogInformation($"Processing configuration for referenceType: {mappingModel.ReferenceType}");

            // Retrieve data from SQL
            // var data = await _sqlDataAccess.GetColumnDataAsync(tableInfo.TableName, tableInfo.ColumnName);


            // Upsert data to Cosmos DB
           // await _cosmosDataAccess.UpsertDocumentAsync(tableInfo.FieldName, data);

            // Retrieve data from SQL using the query in config.SqlQuery
            var data = await _sqlDataAccess.GetDataFromSqlAsync(mappingModel.SqlQuery);

            // Upsert data to Cosmos DB
            await _cosmosDataAccess.UpsertDocumentAsync(mappingModel, data);

        }

        /*
        [FunctionName("LoadConfigurationsActivity")]
        public async Task<List<MappingModel>> Run(
        [ActivityTrigger] string configFilePath,
        ILogger log)
        {
            log.LogInformation($"Loading configurations from {configFilePath}");

            // Load configurations using the ConfigLoader
            var configurations = MappingService.LoadTableConfigurations(configFilePath);

            log.LogInformation($"Loaded {configurations.Count} configurations from JSON.");
            return configurations;
        }
        */

        [FunctionName("DurableDataSyncHttpStarter")]
        public async Task<IActionResult> RunHttpStarter(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
        {
            log.LogInformation("HTTP Starter function triggered.");

            // Start the orchestration
            string instanceId = await starter.StartNewAsync("DurableDataSyncOrchestrator", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
