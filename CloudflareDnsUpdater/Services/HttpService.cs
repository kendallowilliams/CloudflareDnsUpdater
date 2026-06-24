using CloudflareDnsUpdater.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace CloudflareDnsUpdater.Services
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IConfiguration configuration;

        public HttpService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.configuration = configuration;
        }

        public async Task<string> GetIpAddress()
        {
            string ip = string.Empty;

            using (var client = httpClientFactory.CreateClient())
            {
                Uri uri = new Uri(configuration["PublicIpUrl"]);
                var response = await client.GetAsync(uri);

                response.EnsureSuccessStatusCode();
                ip = await response.Content.ReadAsStringAsync();
            }

            return ip;
        }
    }
}
