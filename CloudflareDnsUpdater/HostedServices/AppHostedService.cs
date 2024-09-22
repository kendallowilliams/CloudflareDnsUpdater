using CloudflareDnsUpdater.Models;
using CloudflareDnsUpdater.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

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
                var ipTask = httpService.GetIpAddress();
                var dnsTask = cloudflareService.GetDnsRecords();
                await Task.WhenAll(ipTask, dnsTask);
                var dnsRecords = dnsTask.Result?.DnsRecords.Where(record => record.Type == "A")
                    ?? Enumerable.Empty<DnsRecord>();
                var ipAddress = ipTask.Result;

                if (!dnsRecords.Any(record => record.Content == ipAddress))
                {
                    var record = dnsRecords.First();

                    logger.LogInformation($"Old IP Address: {record.Content}, Latest IP Address: {ipAddress}");
                    record.Content = ipAddress;
                    await cloudflareService.UpdateDnsRecord(record);
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
