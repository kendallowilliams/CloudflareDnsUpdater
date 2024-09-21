using CloudflareDnsUpdater.HostedServices;
using CloudflareDnsUpdater.Models;
using CloudflareDnsUpdater.Services;
using CloudflareDnsUpdater.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Reflection;

namespace GoogleDNSUpdater
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                      .ConfigureAppConfiguration((context, builder) =>
                      {
                          var env = context.HostingEnvironment;

                          if (!env.IsDevelopment())
                          {
                              var assembly = Assembly.Load(new AssemblyName(env.ApplicationName));

                              builder.AddUserSecrets(assembly);
                          }
                      })
                      .ConfigureServices((builder, services) =>
                      {
                          var configuration = builder.Configuration;
                          var cloudflareSettings = configuration.GetSection(nameof(CloudflareSettings)).Get<CloudflareSettings>();

                          services.AddTransient<ICloudflareService, CloudflareService>();
                          services.AddTransient<IHttpService, HttpService>();
                          services.AddHostedService<AppHostedService>();
                          services.Configure<CloudflareSettings>(configuration.GetSection(nameof(CloudflareSettings)));
                          services.AddHttpClient("Cloudflare", httpClient =>
                          {
                              string credentials = $"{cloudflareSettings.ApiKey}";

                              httpClient.BaseAddress = new Uri(cloudflareSettings.EndPoint);
                              httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", credentials);
                              httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                          });
                          services.AddHttpClient();
                          services.AddLogging(builder =>
                          {
                              builder.ClearProviders();
                              builder.AddConsole();
                          });
                      })
                      .Build()
                      .RunAsync();
        }
    }
}
