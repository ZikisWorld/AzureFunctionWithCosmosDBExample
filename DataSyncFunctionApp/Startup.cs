using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos;
using DataSyncFunctionApp.DataAccess;

[assembly: FunctionsStartup(typeof(DataSyncFunctionApp.Startup))]


namespace DataSyncFunctionApp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            // SQL Server DAL registration
            var sqlConnectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            builder.Services.AddSingleton<ISqlDataAccess>(new SqlDataAccess(sqlConnectionString));

            // Cosmos DB DAL registration
            var cosmosEndpoint = Environment.GetEnvironmentVariable("CosmosEndpoint");
            var cosmosKey = Environment.GetEnvironmentVariable("CosmosKey");
            var cosmosDatabaseId = Environment.GetEnvironmentVariable("CosmosDatabaseId");
            var cosmosContainerId = Environment.GetEnvironmentVariable("CosmosContainerId");

            var cosmosClient = new CosmosClient(cosmosEndpoint, cosmosKey);
            builder.Services.AddSingleton<ICosmosDataAccess>(new CosmosDataAccess(cosmosClient, cosmosDatabaseId, cosmosContainerId));
        }
    }
}
