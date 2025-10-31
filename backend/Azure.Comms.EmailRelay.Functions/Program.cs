using Azure.Comms.EmailRelay.Functions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddOptions<EmailQueueOptions>()
            .Bind(context.Configuration.GetSection(EmailQueueOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<BlobStorageOptions>()
            .Bind(context.Configuration.GetSection(BlobStorageOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<CommunicationServiceOptions>()
            .Bind(context.Configuration.GetSection(CommunicationServiceOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
    })
    .Build();

await host.RunAsync();
