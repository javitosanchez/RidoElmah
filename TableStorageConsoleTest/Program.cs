using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableStorageConsoleTest
{

    public class ErrorEntity : TableEntity
    {
        [System.Obsolete("Provided For Serialization From Windows Azure Do No Call Directly")]
        public ErrorEntity()
        {
        }

        /// <summary>
        /// Initialize a new instance of the ErrorEntity class. 
        /// </summary>
        /// <param name="timeUtc"></param>
        /// <param name="Id"></param>
        public ErrorEntity(DateTime timeUtc, Guid Id)
            : base(ErrorEntity.GetPartitionKey(timeUtc), ErrorEntity.GetRowKey(timeUtc, Id))
        {
            this.TimeUtc = timeUtc;
            this.Id = Id;
        }

        /// <summary>
        /// Given a DateTime Return a Parition Key
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetPartitionKey(DateTime time)
        {
            //return time.ToString("yyyyMMddHH");
            return String.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Date.Ticks);
        }

        /// <summary>
        /// Given a Error Identifier Return A Parition Key
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetRowKey(DateTime time, Guid id)
        {
            return String.Format("{0:D19}", DateTime.MaxValue.Ticks - DateTime.UtcNow.Ticks) + id.ToString("N");
        }

        /// <summary>
        /// Unique Error Identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Get or set
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Get or set
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Get or set
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Get or set
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Get or set
        /// </summary>
        public string User { get; set; }

        /// <summary>
        /// Get or set
        /// </summary>
        public DateTime TimeUtc { get; set; }

        /// <summary>
        /// Get or set
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Get or set
        /// </summary>
        public string ErrorXml { get; set; }
    }

  

    class Program
    {
        static void Main(string[] args)
        {
            new Program().ReadWriteFromTable();
        }

        void ReadWriteFromTable()
        {
            var _connectionString = "UseDevelopmentStorage=true";
            var table = CloudStorageAccount.Parse(_connectionString).CreateCloudTableClient();
            CloudTable myTable = table.GetTableReference("MyTable");
            myTable.CreateIfNotExists();

            //InsertItems(myTable);

            //SelectAll(myTable);

            


            TableQuery<ErrorEntity> query = new TableQuery<ErrorEntity>()
                .Where("Id eq guid'fe666de0-ffc3-4e79-a757-d9d7b770e7ad'");

            var results = myTable.ExecuteQuery<ErrorEntity>(query);

            foreach (var item in results)
            {
                Console.WriteLine(item.Id + " "  + item.StatusCode);
            }

        }

        private static void SelectAll(CloudTable myTable)
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<ErrorEntity> query = new TableQuery<ErrorEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.LessThan, DateTime.MaxValue.Ticks.ToString()));

            // Print the fields for each customer.
            foreach (ErrorEntity entity in myTable.ExecuteQuery(query))
            {
                Console.WriteLine("{0}, {1}\t{2}\t{3}", entity.PartitionKey, entity.RowKey,
                    entity.HostName, entity.StatusCode);
            }
        }

        private static void InsertItems(CloudTable myTable)
        {
            for (int i = 0; i < 1000; i++)
            {
                myTable.Execute(
                    TableOperation.Insert(
                        new ErrorEntity(DateTime.Now, Guid.NewGuid())
                        {
                            Id = Guid.NewGuid(),
                            HostName = "rido",
                            Message = "message",
                            StatusCode = i
                        }
                        ));
                Console.Write(".");
            }
        }
    }
}
