using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using TwitterBot.Framework.BusinessLogic;
using TwitterBot.Framework.Contracts;
using TwitterBot.Framework.Mappings;
using TwitterBot.Framework.Types;

namespace TwitterBot.AzureFunctions
{
    public class TweetBotFunction
    {
        private readonly ITweetOperations _tweetOperations;

        public TweetBotFunction(ITweetOperations tweetOperations)
        {
            _tweetOperations = tweetOperations;
        }

        [Function("TweetBotFunction")]
        public async Task Run([TimerTrigger("*/15 * * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("TweetBotFunction");
            var tweet = await _tweetOperations.GetPopularTweetsByHashtagAsync(new Hashtag { Text = "#justsaying" });

            if (tweet != null)
            {
                logger.LogInformation($"Latest popular Tweet for #justsaying : { tweet.FullText }");
            }
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
