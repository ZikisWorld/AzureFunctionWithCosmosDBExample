using DataSyncFunctionApp.Models;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CosmosContainer = Microsoft.Azure.Cosmos.Container;

namespace DataSyncFunctionApp.DataAccess
{
    public class CosmosDataAccess : ICosmosDataAccess
    {
        private readonly CosmosContainer _container;

        public CosmosDataAccess(CosmosClient cosmosClient, string databaseId, string containerId)
        {
            _container = cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task UpsertDocumentAsync(string documentId, IEnumerable<DropdownData> data)
        {
            var document = new
            {
                id = documentId,  // Unique identifier to ensure data replacement
                Items = data
            };
            await _container.UpsertItemAsync(document, new PartitionKey(documentId));
        }
    }
}
