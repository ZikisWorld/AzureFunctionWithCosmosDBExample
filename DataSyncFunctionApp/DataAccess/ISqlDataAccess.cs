using DataSyncFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSyncFunctionApp.DataAccess
{
    public interface ISqlDataAccess
    {
        Task<IEnumerable<DropdownData>> GetColumnDataAsync(string tableName, string columnName);

        Task<IEnumerable<DropdownData>> GetDataFromSqlAsync(string sqlQuery);
    }
}
