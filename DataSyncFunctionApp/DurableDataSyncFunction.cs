using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataSyncFunctionApp.DataAccess;
using DataSyncFunctionApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

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
        [TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,  // Adjust for daily run
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
            var tables = new[]
            {
        new TableInfo { TableName = "Countries", ColumnName = "CountryName", DocumentId = "CountriesDoc" },
        new TableInfo { TableName = "Cars", ColumnName = "CarModel", DocumentId = "CarsDoc" }
    };

            foreach (var table in tables)
            {
                // Call the activity function sequentially for each table
                await context.CallActivityAsync("DurableDataSyncActivity", table);
            }
        }


        [FunctionName("DurableDataSyncActivity")]
        public async Task RunActivity(
    [ActivityTrigger] TableInfo tableInfo,
    ILogger log)
        {
            log.LogInformation($"Processing data for {tableInfo.TableName}");

            // Retrieve data from SQL
            var data = await _sqlDataAccess.GetColumnDataAsync(tableInfo.TableName, tableInfo.ColumnName);

            // Upsert data to Cosmos DB
            await _cosmosDataAccess.UpsertDocumentAsync(tableInfo.DocumentId, data);
        }


        //[FunctionName("DurableDataSyncHttpStarter")]
        //public async Task<IActionResult> RunHttpStarter(
        //[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
        //[DurableClient] IDurableOrchestrationClient starter,
        //ILogger log)
        //{
        //    log.LogInformation("HTTP Starter function triggered.");

        //    // Start the orchestration
        //    string instanceId = await starter.StartNewAsync("DurableDataSyncOrchestrator", null);

        //    log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

        //    return starter.CreateCheckStatusResponse(req, instanceId);
        //}
    }
}
