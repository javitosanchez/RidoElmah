using System.Data.Services.Client;
using Microsoft.WindowsAzure.Storage.Table.DataServices;

namespace Rido.Core.Elmah
{
    internal class TableServiceContextAdapter: ITableServiceContextAdapter
    {
        private readonly TableServiceContext _context = null;

        public TableServiceContextAdapter(TableServiceContext ctx)
        {
            _context = ctx;
        }

        public MergeOption MergeOption
        {
            get
            {
                return _context.MergeOption;
            }
            set
            {
                _context.MergeOption = value;
            }
        }

        public void AddObject(string entitySetName, object entity)
        {
            _context.AddObject(entitySetName, entity);
        }

        public void AttachTo(string entitySetName, object entity)
        {
            _context.AttachTo(entitySetName, entity);
        }

        public void AttachTo(string entitySetName, object entity, string etag)
        {
            _context.AttachTo(entitySetName, entity, etag);
        }

        public DataServiceQuery<T> CreateQuery<T>(string entitySetName)
        {
            return _context.CreateQuery<T>(entitySetName);
        }

        public void DeleteObject(object entity)
        {
            _context.DeleteObject(entity);
        }

        public DataServiceResponse SaveChangesWithRetries()
        {
            return _context.SaveChangesWithRetries();
        }       

        public DataServiceResponse SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
