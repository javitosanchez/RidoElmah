using System;
namespace Rido.Core.Elmah
{
    public interface IErrorEntity
    {
        string ErrorXml { get; set; }
        string HostName { get; set; }
        Guid Id { get; set; }
        string Message { get; set; }
        string Source { get; set; }
        int StatusCode { get; set; }
        DateTime TimeUtc { get; set; }
        string Type { get; set; }
        string User { get; set; }
    }
}
