using DataSyncFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncFunctionApp.DataAccess
{
    public interface ICosmosDataAccess
    {
        Task UpsertDocumentAsync(string documentId, IEnumerable<DropdownData> data);
    }
}
