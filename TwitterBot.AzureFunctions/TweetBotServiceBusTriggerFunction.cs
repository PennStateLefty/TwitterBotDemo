using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using TwitterBot.Framework.Contracts;
using TwitterBot.Framework.Contracts.Data;
using TwitterBot.Framework.Contracts.ServiceBus;
using TwitterBot.Framework.Types;

namespace TwitterBot.AzureFunctions
{
    public class TweetBotServiceBusTriggerFunction
    {
        private readonly ITweetOperations _tweetOperations;
        private readonly IDocumentDbRepository<Tweet> _tweetDocRepo;
        private readonly IDocumentDbRepository<Hashtag> _hashtagRepo;

        public TweetBotServiceBusTriggerFunction(ITweetOperations tweetOperations, IDocumentDbRepository<Tweet> tweetDocRepo, IDocumentDbRepository<Hashtag> hashtagRepo)
        {
            _tweetOperations = tweetOperations;
            _tweetDocRepo = tweetDocRepo;
            _hashtagRepo = hashtagRepo;
        }

        //[Function("TweetBotServiceBusTriggerFunction")]
        public async Task Run([ServiceBusTrigger("hashtagqueue", Connection = "TwitterBotServiceBusConnectionString", IsSessionsEnabled = true)] string myQueueItem, FunctionContext context)
        {
            var logger = context.GetLogger("TweetBotServiceBusTriggerFunction");
            logger.LogInformation($"Entering the TweetBotServiceBusTriggerFunction to poll Twitter API at: {DateTime.Now}");

            var hashtag = JsonConvert.DeserializeObject<Hashtag>(myQueueItem);

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

                logger.LogInformation($"Exiting the TweetBotServiceBusTriggerFunction at: {DateTime.Now}");
            }
            
        }
    }
}
