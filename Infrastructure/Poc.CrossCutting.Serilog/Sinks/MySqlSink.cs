using MySql.Data.MySqlClient;
using Poc.CrossCutting.Serilog.Extensions;
using Serilog.Core;
using Serilog.Debugging;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Poc.CrossCutting.Serilog.Sinks
{
    internal class MySqlSink : BatchProvider, ILogEventSink
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        public MySqlSink(
            string connectionString,
            string tableName = "Logs",
            bool storeTimestampInUtc = false,
            uint batchSize = 100) : base((int)batchSize)
        {
            _connectionString = connectionString;
            _tableName = tableName;
            var sqlConnection = GetSqlConnection();
            CreateTable(sqlConnection);
        }
        public void Emit(LogEvent logEvent)
        {
            PushEvent(logEvent);
        }
        private MySqlConnection GetSqlConnection()
        {
            try
            {
                var conn = new MySqlConnection(_connectionString);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine(ex.Message);
                return null;
            }
        }
        private MySqlCommand GetInsertCommand(MySqlConnection sqlConnection)
        {
            var tableCommandBuilder = new StringBuilder();
            tableCommandBuilder.Append($"INSERT INTO  {_tableName} (");
            tableCommandBuilder.Append("Timestamp, Level, Message, Exception, Properties) ");
            tableCommandBuilder.Append("VALUES (@ts, @lvel, @msg, @ex, @prop)");
            var cmd = sqlConnection.CreateCommand();
            cmd.CommandText = tableCommandBuilder.ToString();
            cmd.Parameters.Add(new MySqlParameter("@ts", MySqlDbType.DateTime));
            cmd.Parameters.Add(new MySqlParameter("@lvel", MySqlDbType.VarChar));
            cmd.Parameters.Add(new MySqlParameter("@msg", MySqlDbType.VarChar));
            cmd.Parameters.Add(new MySqlParameter("@ex", MySqlDbType.VarChar));
            cmd.Parameters.Add(new MySqlParameter("@prop", MySqlDbType.VarChar));
            return cmd;
        }
        private void CreateTable(MySqlConnection sqlConnection)
        {
            try
            {
                var tableCommandBuilder = new StringBuilder();
                tableCommandBuilder.Append($"CREATE TABLE IF NOT EXISTS {_tableName} (");
                tableCommandBuilder.Append("id INT NOT NULL AUTO_INCREMENT PRIMARY KEY,");
                tableCommandBuilder.Append("Timestamp DateTime,");
                tableCommandBuilder.Append("Level VARCHAR(15),");
                tableCommandBuilder.Append("Message TEXT,");
                tableCommandBuilder.Append("Exception TEXT,");
                tableCommandBuilder.Append("Properties TEXT)");
                var cmd = sqlConnection.CreateCommand();
                cmd.CommandText = tableCommandBuilder.ToString();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine(ex.Message);
            }
        }
        protected override async Task<bool> WriteLogEventAsync(ICollection<LogEvent> logEventsBatch)
        {
            try
            {
                using (var sqlCon = GetSqlConnection())
                {
                    using (var tr = await sqlCon.BeginTransactionAsync()
                        .ConfigureAwait(false))
                    {
                        var insertCommand = GetInsertCommand(sqlCon);
                        insertCommand.Transaction = tr;
                        foreach (var logEvent in logEventsBatch)
                        {
                            insertCommand.Parameters["@ts"]
                                .Value = logEvent.Timestamp.DateTime;
                            insertCommand.Parameters["@lvel"]
                                .Value = logEvent.Level.ToString();
                            insertCommand.Parameters["@msg"]
                                .Value = logEvent.MessageTemplate.ToString();
                            insertCommand.Parameters["@ex"]
                                .Value = logEvent.Exception?.ToString();
                            insertCommand.Parameters["@prop"]
                                .Value = logEvent.Properties.Json();
                            await insertCommand.ExecuteNonQueryAsync()
                                .ConfigureAwait(false);
                        }
                        tr.Commit();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                SelfLog.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
