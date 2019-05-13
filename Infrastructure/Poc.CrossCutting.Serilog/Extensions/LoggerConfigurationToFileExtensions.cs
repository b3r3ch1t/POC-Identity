using Serilog;
using Serilog.Configuration;
using Serilog.Debugging;
using System;
using System.IO;
using System.Reflection;

namespace Poc.CrossCutting.Serilog.Extensions
{
    public static class LoggerConfigurationToFileExtensions
    {
        public static LoggerConfiguration SeriLogFile(this LoggerSinkConfiguration loggerConfiguration, RollingInterval rollingInterval = RollingInterval.Day)
        {
            var assemblyName = Assembly.GetExecutingAssembly().Location;
            var pathName = Path.GetDirectoryName(assemblyName);
            var fileName = pathName + @"\log\" + Assembly.GetEntryAssembly().GetName().Name + ".log";
            try
            {
                Directory.CreateDirectory(pathName + @"\log\");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


            if (loggerConfiguration == null)
                throw new ArgumentNullException(nameof(loggerConfiguration));


            try
            {
                return loggerConfiguration.File(fileName, rollingInterval: rollingInterval, shared: true,
                        outputTemplate:
                        "{Timestamp:dd-MM-yyyy HH:mm:ss zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine(ex.Message);
                throw;
            }


        }
    }
}
