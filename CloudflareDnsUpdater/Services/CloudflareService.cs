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

        public async Task<UpdateDnsRecordResponse?> UpdateDnsRecord(DnsRecord dnsRecord)
        {
            string relativePath = $"zones/{settings.ZoneId}/dns_records/{dnsRecord.Id}",
                comment = $"Last Modified: CloudflareDnsUpdater ({DateTime.Now})";
            var content = JsonContent.Create(new { content = dnsRecord.Content, comment });
            var client = httpClientFactory.CreateClient("Cloudflare");
            var response = await client.PatchAsync(relativePath, content);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<UpdateDnsRecordResponse>(jsonSerializerOptions);
        }
    }
}
