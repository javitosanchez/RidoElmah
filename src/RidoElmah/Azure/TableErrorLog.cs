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



namespace Rido.Core.Elmah
{
    public class TableErrorLog : ErrorLog, ITableErrorLog
    {
        private static string _partitionKey = null;
        private static string _rowKey = null;

        private readonly string _tableName = "WebErrors";
        private string _connectionString = null;
        private ITableServiceContextAdapter _context = null;

        private ITableServiceContextAdapter context
        {
            get
            {
                if (_context == null)
                {                    
                    var table = CloudStorageAccount
                                    .Parse(_connectionString)
                                    .CreateCloudTableClient();

                    CloudTable t = table.GetTableReference(_tableName);
                    t.CreateIfNotExists();
                    _context = new TableServiceContextAdapter(table.GetTableServiceContext());
                }
                return _context;
            }
        }
        public string ConnectionString
        {
            get { return _connectionString; }
        }

        public TableErrorLog(ITableServiceContextAdapter context)
        {
            _context = context;
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

            context.AddObject(this._tableName, entity);
            context.SaveChanges();

            return entity.Id.ToString();
        }

        /// <summary>
        /// Get a Error From Windows Azure Storage
        /// </summary>
        /// <param name="id">Error Identifier (Guid)</param>
        /// <returns>Error Fetched (or Null If Not Found)</returns>
        public override ErrorLogEntry GetError(string id)
        {
            var query = from entity in context.CreateQuery<ErrorEntity>(this._tableName)
                        where entity.Id == Guid.Parse(id)
                        select entity;

            ErrorEntity errorEntity = query.FirstOrDefault();
            if (errorEntity == null)
            {
                return null;
            }

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


            var query = this.context
                           .CreateQuery<ErrorEntity>(this._tableName)
                           .Take(pageSize) as DataServiceQuery<ErrorEntity>;

            if (pageIndex != 0)
            {
                query = query.AddQueryOption("NextPartitionKey", TableErrorLog._partitionKey)
                             .AddQueryOption("NextRowKey", TableErrorLog._rowKey);
            }
            var result = query.Execute();

            var response = (QueryOperationResponse)result;

            string nextPartition = null;
            string nextRow = null;
            response.Headers.TryGetValue("x-ms-continuation-NextPartitionKey", out nextPartition);
            response.Headers.TryGetValue("x-ms-continuation-NextRowKey", out nextRow);

            TableErrorLog._partitionKey = nextPartition;
            TableErrorLog._rowKey = nextRow;

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
