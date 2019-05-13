using System;

namespace POC.Domain.Core.Logs
{
    public interface ILoggerInterface : IDisposable
    {
        void Debug(string mensagem);
        void Debug(string mensagem, Exception exception);
        void Erro(Exception exception);
        void Erro(string mensagem, Exception exception);
        void Info(string mensagem);
        void Info(string mensagem, Exception exception);
        void Warn(string mensagem);
        void Warn(string mensagem, Exception exception);
    }
}
