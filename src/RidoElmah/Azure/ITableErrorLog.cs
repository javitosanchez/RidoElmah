using Elmah;
using System;
namespace Rido.Core.Elmah
{
    public interface ITableErrorLog
    {
        ErrorLogEntry GetError(string id);
        int GetErrors(int pageIndex, int pageSize, System.Collections.IList errorEntryList);
        string Log(Error error);
    }
}
