using CloudflareDnsUpdater.Services.Interfaces;
using System.Net;

namespace CloudflareDnsUpdater.Services
{
    public class HttpService : IHttpService
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HttpService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetIpAddress()
        {
            string ip = string.Empty;

            using (var client = httpClientFactory.CreateClient())
            {
                Uri uri = new Uri("http://api.ipify.org/");
                var response = await client.GetAsync(uri);

                response.EnsureSuccessStatusCode();
                ip = await response.Content.ReadAsStringAsync();
            }

            return ip;
        }
    }
}
