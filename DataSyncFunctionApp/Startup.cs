using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using DataSyncFunctionApp.DataAccess;
using Microsoft.Extensions.Configuration;
using System.IO;

[assembly: FunctionsStartup(typeof(DataSyncFunctionApp.Startup))]


namespace DataSyncFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //var config = new ConfigurationBuilder()
            //    .SetBasePath(Directory.GetCurrentDirectory())
            //    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true) // Local overrides
            //    .AddEnvironmentVariables()
            //    .Build();

            // Use IConfiguration to directly access configuration values from environment, local.settings.json, or Azure App Settings

            // Inject IConfiguration as a dependency
            var config = builder.Services.BuildServiceProvider().GetService<IConfiguration>();


            // SQL Server DAL registration
            //var sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            var sqlConnectionString = config["SqlConnectionString"];
            builder.Services.AddSingleton<ISqlDataAccess>(new SqlDataAccess(sqlConnectionString));

            //// Cosmos DB DAL registration
            //var cosmosEndpoint = Environment.GetEnvironmentVariable("CosmosEndpoint");
            //var cosmosKey = Environment.GetEnvironmentVariable("CosmosKey");
            //var cosmosDatabaseId = Environment.GetEnvironmentVariable("CosmosDatabaseId");
            //var cosmosContainerId = Environment.GetEnvironmentVariable("CosmosContainerId");

            // Cosmos DB DAL registration
            var cosmosEndpoint = config["CosmosDb:Endpoint"];
            var cosmosKey = config["CosmosDb:Key"];
            var cosmosDatabaseId = config["CosmosDb:DatabaseId"];
            var cosmosContainerId = config["CosmosDb:ContainerId"];

            var cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);
            builder.Services.AddSingleton<ICosmosDataAccess>(new CosmosDataAccess(cosmosClient, cosmosDatabaseId, cosmosContainerId));

            // Register IConfiguration for injection
            builder.Services.AddSingleton(config);
        }
    }
}
