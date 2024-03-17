using ESG_Survey_Automation.Domain.ConfigModels;
using Google.Cloud.Logging.V2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESG_Survey_Automation.Infrastructure.Logging
{
    public class GCPLoggerProvider : ILoggerProvider
    {
        private readonly GCPConfig _config;
        private readonly LoggingServiceV2Client _client;
        private readonly LogName _logName;
        public GCPLoggerProvider(IConfiguration configuration)
        {
            _config = new(configuration);
            _client = LoggingServiceV2Client.Create();
            _logName = new LogName(_config.ProjectId, "ESG_Survey_Automation");
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new GCPLogger(_client, _logName, categoryName);
        }

        public void Dispose()
        {
            
        }
    }
}
