using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttpClientLogging
{
    public class Worker : BackgroundService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<Worker> logger;
        private readonly string apiPathToMachineEvent = "/api/v1/Alive";

        public Worker(IHttpClientFactory httpClientFactory, ILogger<Worker> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                DummySTO eventToSend = new()
                {
                    AAA = 1,
                    BBB = 100
                };

                try
                {
                    HttpClient httpClient = httpClientFactory.CreateClient("TestApi");
                    HttpResponseMessage response = await httpClient.PostAsJsonAsync(apiPathToMachineEvent, eventToSend, stoppingToken);

                    if (response.IsSuccessStatusCode)
                    {
                    }
                }
                catch (TaskCanceledException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message);
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    public class DummySTO
    {
        public int AAA { get; set; }
        public int BBB { get; set; }
    }
}
