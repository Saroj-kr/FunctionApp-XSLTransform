using System.IO;
using XSLTransformationService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;

[assembly: FunctionsStartup(typeof(XSLTransform.InProcessFA.Startup))]
namespace XSLTransform.InProcessFA
{
    internal class Startup: FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;
            builder.Services.AddLogging();
            builder.Services.AddTransient<ITransformationService, TransformationService>();
            builder.Services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddBlobServiceClient(configuration.GetValue<string>("AzureWebJobsStorage"));
            });
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "local.settings.json"), optional: true, reloadOnChange: true)
                .AddJsonFile(Path.Combine(context.ApplicationRootPath, "settings.json"), optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}
