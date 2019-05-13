using Poc.CrossCutting.Serilog.Extensions;
using POC.Domain.Core.Logs;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;

namespace Poc.CrossCutting.Serilog
{
    public class LoggerInterface : ILoggerInterface
    {

        private readonly Logger _logErro;
        private readonly Logger _logInfo;
        private readonly Logger _logWarn;
        private readonly Logger _logDebug;


        public LoggerInterface(string connectionStringMySql,
            string serilogDsn)
        {

            _logErro = ConfigurarSerilogToSerilog(serilogDsn);

            _logInfo = ConfigurarSerilogToFile();

            _logWarn = ConfigurarSerilogToFile();

            _logDebug = ConfigurarSerilogToMySql(connectionStringMySql);
        }


        public void Dispose()
        {
            _logErro?.Dispose();
            _logInfo?.Dispose();
            _logWarn?.Dispose();
            _logDebug?.Dispose();

            GC.SuppressFinalize(this);
        }


        #region :: Log - ERRO ::

        public void Erro(Exception exception)
        {

            _logErro.Error(exception.Message + exception.Source + exception.StackTrace);
        }

        public void Erro(string mensagem, Exception exception)
        {
            _logErro.Error(mensagem, exception);
        }

        #endregion

        #region :: Log - WARN ::


        public void Warn(string mensagem)
        {
            _logWarn.Warning(mensagem);
        }


        public void Warn(string mensagem, Exception exception)
        {
            _logWarn.Warning(mensagem, exception);
        }

        #endregion

        #region :: Log - INFO ::

        public void Info(string mensagem)
        {
            _logInfo.Information(mensagem);
        }

        public void Info(string mensagem, Exception exception)
        {
            _logInfo.Information(mensagem, exception);
        }

        #endregion

        #region :: Log - DEBUG ::

        /// <summary>
        /// Log de info
        /// </summary>
        /// <param name="mensagem">Mensagem</param>
        public void Debug(string mensagem)
        {
            _logDebug.Debug(mensagem);
        }


        public void Debug(string mensagem, Exception e)
        {
            _logDebug.Debug(exception: e, messageTemplate: mensagem);
        }

        #endregion


        private Logger ConfigurarSerilogToMySql(string connectionString)
        {

            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.SeriLogToMySql(connectionString)
                .CreateLogger();

            return logger;
        }


        private Logger ConfigurarSerilogToFile(RollingInterval rollingInterval = RollingInterval.Day)
        {

            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.SeriLogFile(rollingInterval)
                .CreateLogger();

            return logger;
        }


        private Logger ConfigurarSerilogToSerilog(string serilogDsn)
        {

            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Sentry(dsn: serilogDsn)
                .CreateLogger();

            return logger;
        }


    }
}

