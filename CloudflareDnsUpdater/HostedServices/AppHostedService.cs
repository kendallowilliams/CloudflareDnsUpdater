using CloudflareDnsUpdater.Services.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CloudflareDnsUpdater.HostedServices
{
    public class AppHostedService : IHostedService
    {
        private readonly IProcessorService processorService;
        private readonly ILogger<AppHostedService> logger;
        private readonly IHostApplicationLifetime hostApplicationLifetime;

        public AppHostedService(
            ILogger<AppHostedService> logger,
            IHostApplicationLifetime hostApplicationLifetime,
            IProcessorService processorService)
        {
            this.processorService = processorService;
            this.logger = logger;
            this.hostApplicationLifetime = hostApplicationLifetime;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation($"{nameof(CloudflareDnsUpdater)}->{nameof(StartAsync)} Started: {DateTime.Now}");
            try
            {
                await processorService.RunDynamicDnsUpdater();
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
