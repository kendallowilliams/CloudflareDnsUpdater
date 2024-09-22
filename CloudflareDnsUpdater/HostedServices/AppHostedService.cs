using CloudflareDnsUpdater.Exceptions;
using CloudflareDnsUpdater.Models;
using CloudflareDnsUpdater.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using static CloudflareDnsUpdater.Enums;

namespace CloudflareDnsUpdater.HostedServices
{
    public class AppHostedService : IHostedService
    {
        private readonly ICloudflareService cloudflareService;
        private readonly IHttpService httpService;
        private readonly ILogger<AppHostedService> logger;
        private readonly IHostApplicationLifetime hostApplicationLifetime;

        public AppHostedService(
            ICloudflareService cloudflareService, 
            IHttpService httpService, 
            ILogger<AppHostedService> logger,
            IHostApplicationLifetime hostApplicationLifetime)
        {
            this.cloudflareService = cloudflareService;
            this.httpService = httpService;
            this.logger = logger;
            this.hostApplicationLifetime = hostApplicationLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{nameof(CloudflareDnsUpdater)}->{nameof(StartAsync)} Started: {DateTime.Now}");
            try
            {
                var ipAddress = await httpService.GetIpAddress();
                var getDnsRecordsResponse = await cloudflareService.GetDnsRecords();

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
                var dnsRecord = dnsRecords.FirstOrDefault() ?? new DnsRecord() { Content = ipAddress };

                logger.LogInformation($"Old IP Address: {dnsRecord?.Content}, Latest IP Address: {ipAddress}");

                if (needsUpdate)
                {
                    var updateDnsRecordResponse = await cloudflareService.UpdateDnsRecord(dnsRecord);

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
            catch(Exception ex)
            {
                logger.LogError(ex, null);
            }
            finally
            {
                hostApplicationLifetime.StopApplication();
            }
            logger.LogInformation($"{nameof(CloudflareDnsUpdater)}->{nameof(StartAsync)} Ended: {DateTime.Now}");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
