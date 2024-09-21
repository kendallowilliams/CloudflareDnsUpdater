namespace CloudflareDnsUpdater.Services.Interfaces
{
    public interface IHttpService
    {
        Task<string> GetIpAddress();
    }
}
