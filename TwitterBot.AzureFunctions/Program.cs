using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Configuration;
using System;
using System.Threading.Tasks;

using TwitterBot.Framework.BusinessLogic;
using TwitterBot.Framework.Contracts;
using TwitterBot.Framework.Contracts.Data;
using TwitterBot.Framework.Contracts.ServiceBus;
using TwitterBot.Framework.CosmosDB;
using TwitterBot.Framework.ServiceBus;

namespace TwitterBot.AzureFunctions
{
    public class Program
    {
        public static void Main()
        {
            //Try out Configuration driven by Azure KeyVault
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddAzureKeyVault(new Uri("https://twitterbotdemokeyvault.vault.azure.net/"), new DefaultAzureCredential());
            builder.AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();

			Console.WriteLine($"TwitterAPIKey retrieved from KeyVault: { configuration["TwitterAPIKey"] }");

            //Configure the CosmosDB context
            var documentDbContext = new DocumentDbContext
            {
                AuthKey = configuration["TwitterBotDBAuthKey"],
                EndpointUri = configuration["TwitterBotDbUri"],
                DatabaseId = "TwitterBotDB"
            };
            Task.Run(async () => await documentDbContext.CreateDatabaseAndCollectionsAsync()).Wait();

            //Configure the Azure Service Bus context
            var serviceBusContext = new ServiceBusContext()
            {
                ConnectionString = configuration["TwitterBotServiceBusConnectionString"],
                QueueName = "HashTagQueue",
                MaxConcurrentMessagesToBeRetrieved = 2,
                SessionId = "TwitterBotApplication",
                OperationTimeout = TimeSpan.FromMilliseconds(500)
            };
            
            var host = new HostBuilder()               
                .ConfigureAppConfiguration(c =>
                {
                    c.AddEnvironmentVariables();
                    //c.AddAzureKeyVault(new Uri("https://twitterbotdemokeyvault.vault.azure.net/"), new DefaultAzureCredential());
                    c.Build();
                })
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s => {
                    s.AddSingleton<ITweetOperations>(new TweetOperations(configuration["TwitterAPIKey"], configuration["TwitterAPISecret"], configuration["TwitterAPIBearerToken"]))
                    .AddSingleton<IConfiguration>(configuration)
                    .AddSingleton<IDocumentDbContext>(documentDbContext)
                    .AddSingleton(typeof(IDocumentDbRepository<>), typeof(DocumentDbRepository<>))
                    .AddSingleton<IServiceBusContext>(serviceBusContext)
                    .AddSingleton<IServiceBusOperations>(new ServiceBusOperations(serviceBusContext));
                })
                .Build();

            host.Run();
        }
    }
}