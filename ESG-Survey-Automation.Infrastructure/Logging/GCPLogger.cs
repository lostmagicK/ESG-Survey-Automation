using Google.Api;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESG_Survey_Automation.Infrastructure.Logging
{
    public class GCPLogger : ILogger
    {
        private readonly LoggingServiceV2Client _client;
        private readonly LogName _logName;
        public GCPLogger(LoggingServiceV2Client client, LogName logName, string categoryName)
        {
            _client = client;
            _logName = logName;
        }
        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            LogEntry logEntry = new()
            {
                LogName = _logName.ToString(),
                Timestamp = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
                InsertId = eventId.Name,
            };
            if (exception != null)
            {
                logEntry.SourceLocation = LogEntrySourceLocation.Parser.ParseFrom(Encoding.ASCII.GetBytes(exception.StackTrace ?? ""));
                logEntry.LogNameAsLogName = _logName;
                logEntry.Severity = GetGCPSeverity(logLevel);
                
            }
        }

        public static LogSeverity GetGCPSeverity(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Trace => LogSeverity.Default,
                LogLevel.Debug => LogSeverity.Debug,
                LogLevel.Information => LogSeverity.Info,
                LogLevel.Warning => LogSeverity.Warning,
                LogLevel.Error => LogSeverity.Error,
                LogLevel.Critical => LogSeverity.Critical,
                _ => LogSeverity.Notice,
            };
        }

    }
}
