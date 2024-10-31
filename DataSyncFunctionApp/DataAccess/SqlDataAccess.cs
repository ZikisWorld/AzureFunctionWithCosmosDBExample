using DataSyncFunctionApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;
using Microsoft.Data.SqlClient;

namespace DataSyncFunctionApp.DataAccess
{
    public class SqlDataAccess : ISqlDataAccess
    {
        private readonly string _connectionString;

        public SqlDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }
        public async Task<IEnumerable<DropdownData>> GetColumnDataAsync(string tableName, string columnName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = $"SELECT {columnName} AS Name FROM {tableName}";
                return await connection.QueryAsync<DropdownData>(query);
            }
        }
    }
}
