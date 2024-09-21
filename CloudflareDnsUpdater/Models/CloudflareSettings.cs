namespace CloudflareDnsUpdater.Models
{
    public class CloudflareSettings
    {
        public CloudflareSettings() { }

        public required string EndPoint { get; set; }
        public required string ApiKey { get; set; }
        public required string ZoneId { get; set; }
    }
}
