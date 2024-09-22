using CloudflareDnsUpdater.Models;

namespace CloudflareDnsUpdater.Services.Interfaces
{
    public interface ICloudflareService
    {
        Task<ListDnsRecordsResponse?> GetDnsRecords(string zoneId);
        Task<UpdateDnsRecordResponse?> UpdateDnsRecord(string zoneId, DnsRecord dnsRecord);
    }
}
