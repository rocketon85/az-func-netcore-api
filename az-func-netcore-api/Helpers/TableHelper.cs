using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace az_func_netcore_api.Helpers
{
    internal class TableHelper<TElement> where TElement : ITableEntity, new()
    {
        private CloudTable GetCloudTable(string tableName)
        {
            var conexionTable = Environment.GetEnvironmentVariable("ConnectionTable");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(conexionTable);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);

            return table;
        }
        public TElement GetEntity(string tableName, TableQuery<TElement> query)
        {
            var table = GetCloudTable(tableName);

            var data = table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken()).GetAwaiter().GetResult();
            if (data?.Results.Count == 1) return data.Results[0];
            return default;
        }

        public List<TElement> GetAllEntity(string tableName, TableQuery<TElement> query)
        {
            var table = GetCloudTable(tableName);

            var data = table.ExecuteQuerySegmentedAsync(query, new TableContinuationToken()).GetAwaiter().GetResult();
            return data.Results;
        }
    }
}
