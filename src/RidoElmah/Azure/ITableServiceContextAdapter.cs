using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Services.Client;

namespace Rido.Core.Elmah
{
    public interface ITableServiceContextAdapter
    {
        MergeOption MergeOption { get; set; }

        void AddObject(string entitySetName, object entity);
        void AttachTo(string entitySetName, object entity);
        void AttachTo(string entitySetName, object entity, string etag);
        DataServiceQuery<T> CreateQuery<T>(string entitySetName);
        void DeleteObject(object entity);
        DataServiceResponse SaveChangesWithRetries();
        DataServiceResponse SaveChanges();
    }
}
