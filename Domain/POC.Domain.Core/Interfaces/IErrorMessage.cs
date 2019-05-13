namespace POC.Domain.Core.Interfaces
{
    public interface IErrorMessage
    {
        int StatusCode { get; }
        string StatusDescription { get; }
        string Message { get; }
    }
}