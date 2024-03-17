using Microsoft.Extensions.Configuration;

namespace ESG_Survey_Automation.Domain.ConfigModels
{
    public class GCPConfig
    {
        public GCPConfig(IConfiguration configuration)
        {
            var config = configuration.GetSection("GCPConfig");
            ProjectId = config["ProjectId"];
            StorageBucket = config["StorageBucket"];
        }
        public string ProjectId { get; set; }
        public string StorageBucket { get; set; }
    }
}
