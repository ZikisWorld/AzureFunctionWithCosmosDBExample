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

        public async Task UpsertDocumentAsync(MappingModel mappingModel , IEnumerable<DropdownData> data)
        {
            // Use a fixed id based on the field name to ensure uniqueness
           // var documentId = $"{fieldNm}-document";

            var document = new
            {
                id = mappingModel.DocumentId,
                fieldName = mappingModel.PartitionValue,  // Partition Value
                Items = data.Select(d => d.Name)
            };
            await _container.UpsertItemAsync(document, new PartitionKey(mappingModel.PartitionValue));
        }
    }
}
