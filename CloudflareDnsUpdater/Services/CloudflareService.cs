using CloudflareDnsUpdater.Models;
using CloudflareDnsUpdater.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace CloudflareDnsUpdater.Services
{
    public class CloudflareService : ICloudflareService
    {
        private readonly CloudflareSettings settings;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly JsonSerializerOptions jsonSerializerOptions;
        private readonly ILogger<CloudflareService> logger;

        public CloudflareService(
            IOptions<CloudflareSettings> settings, 
            IHttpClientFactory httpClientFactory, 
            ILogger<CloudflareService> logger)
        {
            this.settings = settings.Value;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ListDnsRecordsResponse?> GetDnsRecords()
        {
            string relativePath = $"zones/{settings.ZoneId}/dns_records";
            var client = httpClientFactory.CreateClient("Cloudflare");
            var response = await client.GetAsync(relativePath);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<ListDnsRecordsResponse>(jsonSerializerOptions);
        }

        public async Task UpdateDnsRecord(DnsRecord record)
        {
            string relativePath = $"zones/{settings.ZoneId}/dns_records/{record.Id}";
            var content = JsonContent.Create(new { content = record.Content });
            var client = httpClientFactory.CreateClient("Cloudflare");
            var response = await client.PatchAsync(relativePath, content);

            response.EnsureSuccessStatusCode();
        }
    }
}
