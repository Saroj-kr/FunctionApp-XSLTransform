using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;

namespace FunctionApp.XSLTransform;

public class Program
{
    public static void Main()
    {
        try
        {
            var host = new HostBuilder()
            .ConfigureFunctionsWebApplication()
            .ConfigureAppConfiguration(builder => { })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddApplicationInsightsTelemetryWorkerService();
                services.ConfigureFunctionsApplicationInsights();
                services.AddTransient<ITransformationClient, TransformationClient>();
                services.AddAzureClients(clientBuilder =>
                {
                    clientBuilder.AddBlobServiceClient(hostContext.Configuration.GetValue<string>("AzureWebJobsStorage"));
                });
            })
            .Build();

            host.Run();
        }
        catch (Exception)
        {
            throw;
        }
    }
}