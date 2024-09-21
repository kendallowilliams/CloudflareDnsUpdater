using System.Text.Json.Serialization;

namespace CloudflareDnsUpdater.Models
{
    public class DnsRecord
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Content { get; set; }
        public int TTL { get; set; }
        public string? Type { get; set; }
    }
}
