namespace CloudflareDnsUpdater.Services.Interfaces
{
    public interface IProcessorService
    {
        Task RunDynamicDnsUpdater();
    }
}
