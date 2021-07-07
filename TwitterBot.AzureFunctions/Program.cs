using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Configuration;

using TwitterBot.Framework.BusinessLogic;
using TwitterBot.Framework.Contracts;

namespace TwitterBot.AzureFunctions
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(s => {
                    s.AddSingleton<ITweetOperations>(new TweetOperations("Dafj6W6zArpa0CGaGki4Sc5uS", "0JxlLQ2b9Fqd4yzeiHRXEBqzdnCzDmBA2JQo1Di9KNF9aOAAMx", @"AAAAAAAAAAAAAAAAAAAAAPOQRQEAAAAAzqLSsSfaHH8a2hnaQSboMTbEiZA%3DwgYNEjqFTFYFIGxoOgrpMMTYLuWu1TOe2ZzMNrYVxYypxIMto7"));
                })
                .Build();

            host.Run();
        }
    }
}