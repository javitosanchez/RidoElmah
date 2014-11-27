using Rido.Core.Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Services.Client;

namespace RidoElmah
{
    //http://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-tables/#configure-access
    internal class AzStorageTableAdapter : ITableServiceContextAdapter
    {
        public MergeOption MergeOption
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void AddObject(string entitySetName, object entity)
        {
            throw new NotImplementedException();
        }

        public void AttachTo(string entitySetName, object entity)
        {
            throw new NotImplementedException();
        }

        public void AttachTo(string entitySetName, object entity, string etag)
        {
            throw new NotImplementedException();
        }

        public DataServiceQuery<T> CreateQuery<T>(string entitySetName)
        {
            throw new NotImplementedException();
        }

        public void DeleteObject(object entity)
        {
            throw new NotImplementedException();
        }

        public DataServiceResponse SaveChanges()
        {
            throw new NotImplementedException();
        }

        public DataServiceResponse SaveChangesWithRetries()
        {
            throw new NotImplementedException();
        }
    }
}
