using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Elmah;
using System.Collections;
using Microsoft.WindowsAzure;
//using Microsoft.WindowsAzure.ServiceRuntime;
using System.Data.Services.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;



namespace RidoElmah.Azure
{
    public class TableErrorLog : ErrorLog
    {
        private static string _partitionKey = null;
        private static string _rowKey = null;

        private readonly string _tableName = "WebErrors";
        private string _connectionString = null;
        private CloudTable _table= null;
                

        private CloudTable table
        {
            get
            {
                if (_table == null)
                {                    
                    var table = CloudStorageAccount
                                    .Parse(_connectionString)
                                    .CreateCloudTableClient();

                    CloudTable tableWebErrors = table.GetTableReference(_tableName);
                    tableWebErrors.CreateIfNotExists();

                    _table = tableWebErrors;
                }
                return _table;
            }
        }
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        /// <summary>
        /// Initialize a new instance of the WindowsAzureErrorLogs class.
        /// </summary>
        /// <param name="config"></param>
        public TableErrorLog(IDictionary config)
        {
            if (!(config["connectionString"] is string))
            {
                throw new ArgumentException("El Valor no puede estar a nulo o vacío.", "connectionString");
            }

            if (string.IsNullOrWhiteSpace((string)config["connectionString"]))
            {
                throw new ArgumentException("El Valor no puede estar a nulo o vacío.", "connectionString");
            }

            _connectionString = (string)config["connectionString"];
        
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public override string Log(Error error)
        {
            ErrorEntity entity = new ErrorEntity(error.Time, Guid.NewGuid())
            {
                HostName = error.HostName,
                Type = error.Type,
                ErrorXml = ErrorXml.EncodeString(error),
                Message = error.Message,
                StatusCode = error.StatusCode,
                User = error.User,
                Source = error.Source
            };

            TableOperation insert = TableOperation.Insert(entity, true);
            table.Execute(insert);            
            return entity.Id.ToString();
        }

        /// <summary>
        /// Get a Error From Windows Azure Storage
        /// </summary>
        /// <param name="id">Error Identifier (Guid)</param>
        /// <returns>Error Fetched (or Null If Not Found)</returns>
        public override ErrorLogEntry GetError(string id)
        {
            TableQuery<ErrorEntity> query = new TableQuery<ErrorEntity>().Where(string.Format("Id eq guid'{0}'", id));

            var results = table.ExecuteQuery(query);
            
            if (results.Count() <1 )
            {
                return null;
            }

            var errorEntity = results.FirstOrDefault();

            return new ErrorLogEntry(this, id, ErrorXml.DecodeString(errorEntity.ErrorXml));
        }

        /// <summary>
        /// Get A Page Of Errors From Windows Azure Storage
        /// </summary>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Size Of Page To Return</param>
        /// <param name="errorEntryList">List of Errors Returned</param>
        /// <returns>Total Count of Errors</returns>
        public override int GetErrors(int pageIndex, int pageSize, System.Collections.IList errorEntryList)
        {
            if (pageIndex < 0)
                throw new ArgumentOutOfRangeException("pageIndex", pageIndex, null);

            if (pageSize < 0)
                throw new ArgumentOutOfRangeException("pageSize", pageSize, null);


            /*
            var query = this.table
                           .CreateQuery<ErrorEntity>(this._tableName)
                           .Take(pageSize) as DataServiceQuery<ErrorEntity>;
                           */

            TableQuery<ErrorEntity> query;
            
            
                query = new TableQuery<ErrorEntity>()
                .Where(TableQuery.GenerateFilterCondition(
                                "PartitionKey",
                                QueryComparisons.LessThanOrEqual,
                                DateTime.MaxValue.Ticks.ToString()))
                .Take(pageSize);

            var result = table.ExecuteQuery<ErrorEntity>(query);

            var last = result.OrderByDescending(e => e.RowKey).FirstOrDefault();

            if (pageIndex > 0)
            {
                query = new TableQuery<ErrorEntity>()
                    .Where(TableQuery.CombineFilters(
                            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThan, last.PartitionKey),
                            TableOperators.And,
                            TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.LessThan, last.RowKey)
                        ))
                    .Take(pageSize);
                    result = table.ExecuteQuery<ErrorEntity>(query);
            }
            
            /*

            var response = (QueryOperationResponse)result;

            string nextPartition = null;
            string nextRow = null;
            response.Headers.TryGetValue("x-ms-continuation-NextPartitionKey", out nextPartition);
            response.Headers.TryGetValue("x-ms-continuation-NextRowKey", out nextRow);

            TableErrorLog._partitionKey = nextPartition;
            TableErrorLog._rowKey = nextRow;

            */

            foreach (var item in result)
            {
                errorEntryList.Add( new ErrorLogEntry(this, item.Id.ToString(), ErrorXml.DecodeString(item.ErrorXml)));                                
            };
            
            

            int count = 0;
            if (errorEntryList.Count == pageSize)
                count = ((pageIndex + 1) * pageSize) + 1;
            else
                count = (pageIndex * pageSize) + errorEntryList.Count;

            return count;
        }

        public static void Raise(Exception ex)
        {
            if (HttpContext.Current != null)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
        }
    }
}
