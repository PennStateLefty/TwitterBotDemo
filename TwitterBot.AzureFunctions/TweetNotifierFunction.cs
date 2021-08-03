using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TwitterBot.AzureFunctions
{
    public static class TweetNotifierFunction
    {
        [Function("TweetNotifierFunction")]
        public static void Run([CosmosDBTrigger(
            databaseName: "TwitterBotDB",
            collectionName: "TweetCollection",
            ConnectionStringSetting = "TwitterBotDbConnectionString",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<MyDocument> input, FunctionContext context)
        {
            var logger = context.GetLogger("TweetNotifierFunction");
            if (input != null && input.Count > 0)
            {
                logger.LogInformation("Documents modified: " + input.Count);
                logger.LogInformation("First document Id: " + input[0].Id);
            }
        }
    }

    public class MyDocument
    {
        public string Id { get; set; }

        public string Text { get; set; }

        public int Number { get; set; }

        public bool Boolean { get; set; }
    }
}
