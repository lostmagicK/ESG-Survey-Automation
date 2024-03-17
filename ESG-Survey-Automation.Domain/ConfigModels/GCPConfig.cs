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
            Folder = config["Folder"];
            ClientId = config["ClientId"];
            ClientSecret = config["ClientSecret"];
            AuthUrl = config["AuthUrl"];
            TokenUrl = config["TokenUrl"];
        }
        public string ProjectId { get; set; }
        public string StorageBucket { get; set; }
        public string Folder { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AuthUrl { get; set; }
        public string TokenUrl { get; set; }
    }
}
