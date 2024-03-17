using ESG_Survey_Automation.Domain.ConfigModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Google.Cloud.Storage.V1;

namespace ESG_Survey_Automation.Infrastructure.FileStorage
{
    public class GCPFileStorage : IFileStorage
    {
        private readonly GCPConfig _config;
        public readonly StorageClient _client;
        public GCPFileStorage(IConfiguration configuration)
        {
            _config = new(configuration);
            _client = StorageClient.Create();
        }
        public async Task UploadFileToCloud(IFormFile file)
        {
            await _client.UploadObjectAsync(_config.StorageBucket, $"{_config.Folder}/{Guid.NewGuid()}.pdf", "application/pdf", file.OpenReadStream());
        }
    }
}
