using CloudflareDnsUpdater.Models;

namespace CloudflareDnsUpdater.Services.Interfaces
{
    public interface ICloudflareService
    {
        Task<ListDnsRecordsResponse?> GetDnsRecords();
        Task UpdateDnsRecord(DnsRecord record);
    }
}
