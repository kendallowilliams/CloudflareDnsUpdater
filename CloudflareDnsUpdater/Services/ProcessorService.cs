using CloudflareDnsUpdater.Exceptions;
using CloudflareDnsUpdater.Models;
using CloudflareDnsUpdater.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using static CloudflareDnsUpdater.Enums;

namespace CloudflareDnsUpdater.Services
{
    public class ProcessorService : IProcessorService
    {
        private readonly CloudflareSettings settings;
        private readonly ILogger<ProcessorService> logger;
        private readonly ICloudflareService cloudflareService;
        private readonly IHttpService httpService;

        public ProcessorService(
            IOptions<CloudflareSettings> settings, 
            ILogger<ProcessorService> logger,
            ICloudflareService cloudflareService,
            IHttpService httpService)
        {
            this.cloudflareService = cloudflareService;
            this.httpService = httpService;
            this.settings = settings.Value;
            this.logger = logger;
        }

        public async Task RunDynamicDnsUpdater()
        {
            var ipAddress = await httpService.GetIpAddress();
            var getDnsRecordsResponse = await cloudflareService.GetDnsRecords(settings.ZoneId);

            if (getDnsRecordsResponse is null)
            {
                throw new CloudflareException("GetDnsRecords failed for an unknown reason.");
            }

            if (!getDnsRecordsResponse.Success)
            {
                throw new CloudflareException(JsonSerializer.Serialize(getDnsRecordsResponse.Errors));
            }

            var dnsRecords = getDnsRecordsResponse.DnsRecords.Where(record => record.Type == DnsRecordTypes.A);
            var needsUpdate = dnsRecords.All(dnsRecord => dnsRecord.Content != ipAddress);
            var ipAddresses = string.Join(", ", dnsRecords.Select(dnsRecord => dnsRecord.Content));

            logger.LogInformation($"IP Address(es): {ipAddresses}, Latest IP Address: {ipAddress}");

            if (needsUpdate)
            {
                var dnsRecord = dnsRecords.FirstOrDefault() ?? new DnsRecord() { Content = ipAddress };
                var updateDnsRecordResponse = await cloudflareService.UpdateDnsRecord(settings.ZoneId, dnsRecord);

                if (updateDnsRecordResponse is null)
                {
                    throw new CloudflareException("UpdateDnsRecord failed for an unknown reason.");
                }

                if (!updateDnsRecordResponse.Success)
                {
                    throw new CloudflareException(JsonSerializer.Serialize(updateDnsRecordResponse.Errors));
                }
            }
        }
    }
}
