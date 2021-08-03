using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitterBot.Framework.Contracts;
using TwitterBot.Framework.Contracts.Data;
using TwitterBot.Framework.Contracts.ServiceBus;
using TwitterBot.Framework.Types;

namespace TwitterBot.AzureFunctions
{
    public class TweetBotFunction
    {
        private readonly ITweetOperations _tweetOperations;
        private readonly IDocumentDbRepository<Tweet> _tweetDocRepo;
        private readonly IDocumentDbRepository<Hashtag> _hashtagRepo;
        private readonly IServiceBusOperations _serviceBusOperations;

        public TweetBotFunction(ITweetOperations tweetOperations, IDocumentDbRepository<Tweet> tweetDocRepo, IDocumentDbRepository<Hashtag> hashtagRepo, IServiceBusOperations serviceBusOperations)
        {
            _tweetOperations = tweetOperations;
            _tweetDocRepo = tweetDocRepo;
            _hashtagRepo = hashtagRepo;
            _serviceBusOperations = serviceBusOperations;
        }

        [Function("TweetBotFunction")]
        public async Task Run([TimerTrigger("20 * * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("TweetBotFunction");
            logger.LogInformation($"Entering the TweetBotFunction to poll Twitter API at: {DateTime.Now}");

            var hashtagMessages = await _serviceBusOperations.ReceiveMessagesAsync();
            var hashtags = hashtagMessages.Select(p => JsonConvert.DeserializeObject<Hashtag>(Encoding.UTF8.GetString(p.Body)));
            foreach (Hashtag hashtag in hashtags)
			{
                logger.LogInformation($"Making a call to the Twitter API - Retrieve Tweets matching {hashtag.Text}");
                var tweet = await _tweetOperations.GetPopularTweetsByHashtagAsync(hashtag);

                if (tweet != null)
                {
                    tweet.HashTags = new List<Hashtag>();
                    logger.LogInformation($"Latest popular tweet for #{hashtag.Text}: {tweet.FullText}");

                    //Check to see if the Tweet is already in the DocumentCollection
                    var existingTweet = await _tweetDocRepo.GetByIdAsync(tweet.Id);
                    if (existingTweet == null)
                    {
                        tweet.HashTags.Add(hashtag);
                        logger.LogInformation("Persisting the Tweet to the database");
                        await _tweetDocRepo.AddOrUpdateAsync(tweet);
                        logger.LogInformation($"Added Tweet in TweetCollection with id: { tweet.Id }");
                    }

                    //Map DB hashtags with latest tweet
                    if (existingTweet != null && !existingTweet.HashTags.Any(p => p.Text == hashtag.Text))
                    {
                        tweet.HashTags = existingTweet.HashTags;
                        tweet.HashTags.Add(hashtag);
                        await _tweetDocRepo.AddOrUpdateAsync(tweet);
                        logger.LogInformation($"Updated Tweet in TweetCollection with Id: {tweet.Id}");
                    }

                    //Update the Hashtag record in the database with the latest in queue timestamp
                    hashtag.IsCurrentlyInQueue = true;
                    hashtag.LastSyncDateTime = DateTime.UtcNow;
                    await _hashtagRepo.AddOrUpdateAsync(hashtag);
                }
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
