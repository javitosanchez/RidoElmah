using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Microsoft.WindowsAzure.StorageClient;
using Elmah;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.Storage.Table;

namespace Rido.Core.Elmah
{
    public class ErrorEntity : TableEntity, IErrorEntity
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
}
