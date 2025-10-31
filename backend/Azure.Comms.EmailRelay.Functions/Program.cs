using Azure.Comms.EmailRelay.Functions.Configuration;
using Azure.Communication.Email;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Azure.Comms.EmailRelay.Functions.Services;
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

        services.AddSingleton(sp =>
        {
            var queueConnectionString = context.Configuration["AzureWebJobsStorage"]
                ?? throw new InvalidOperationException("AzureWebJobsStorage connection string is not configured.");
            return new QueueServiceClient(queueConnectionString);
        });

        services.AddSingleton(sp =>
        {
            var attachmentOptions = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<BlobStorageOptions>>().Value;
            return new BlobServiceClient(attachmentOptions.ConnectionString);
        });

        services.AddSingleton(sp =>
        {
            var commsOptions = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<CommunicationServiceOptions>>().Value;
            return new EmailClient(commsOptions.ConnectionString);
        });

        services.AddSingleton<IEmailMetricsRecorder, EmailMetricsRecorder>();
        services.AddSingleton<IAttachmentLoader, AttachmentLoader>();
        services.AddSingleton<IEmailDispatcher, EmailDispatcher>();
        services.AddSingleton<IFailureQueuePublisher, FailureQueuePublisher>();
        services.AddSingleton<IOutboxQueuePublisher, OutboxQueuePublisher>();
        services.AddSingleton<IFailureArchiveWriter, FailureArchiveWriter>();
    })
    .Build();

await host.RunAsync();
