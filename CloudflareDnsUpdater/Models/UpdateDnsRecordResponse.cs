using System.Text.Json.Serialization;

namespace CloudflareDnsUpdater.Models
{
    public class UpdateDnsRecordResponse
    {
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();

        public IEnumerable<string> Messages { get; set; } = Enumerable.Empty<string>();

        public bool Success { get; set; }

        [JsonPropertyName("result")]
        public required DnsRecord DnsRecord { get; set; }
    }
}
