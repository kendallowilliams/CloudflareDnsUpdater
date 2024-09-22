using System.Text.Json.Serialization;
using static CloudflareDnsUpdater.Enums;

namespace CloudflareDnsUpdater.Models
{
    public class DnsRecord
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Content { get; set; }
        public int TTL { get; set; }
        public string? Comment { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DnsRecordTypes? Type { get; set; }
    }
}
