using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TwitterBot.Framework.Contracts.Data;
using TwitterBot.Framework.Contracts.ServiceBus;
using TwitterBot.Framework.Types;

namespace TwitterBot.AzureFunctions
{
    public class TweetSchedulerFunction
    {
        private readonly IServiceBusOperations _serviceBusOperations;
        private readonly IDocumentDbRepository<Hashtag> _hashtagRepository;

        public TweetSchedulerFunction(IServiceBusOperations serviceBusOperations, IDocumentDbRepository<Hashtag> documentDBRepository)
		{
            _serviceBusOperations = serviceBusOperations;
            _hashtagRepository = documentDBRepository;
		}

        [Function("TweetSchedulerFunction")]
        public async Task Run([TimerTrigger("0 */1 * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("TweetSchedulerFunction");
            logger.LogInformation($"TweetSchedulerFunction started execution at: {DateTime.Now}");
            //Get Hashtags from the CosmosDB
            var hashTags = await _hashtagRepository.WhereAsync(p => (!p.IsCurrentlyInQueue && p.LastSyncDateTime < DateTime.UtcNow.AddMinutes(-10)) || (p.IsCurrentlyInQueue && p.LastSyncDateTime < DateTime.UtcNow.AddHours(-1)));
            foreach (var hashTag in hashTags)
			{
                //Queue the hashtags
                await _serviceBusOperations.SendMessagesAsync(hashTag.Id, JsonConvert.SerializeObject(hashTag));
                //Mark as in queue and update DB
                hashTag.IsCurrentlyInQueue = true;
                await _hashtagRepository.AddOrUpdateAsync(hashTag);
			}

            logger.LogInformation($"TweetSchedulerFunction ended invocation at: {DateTime.Now}");
        }
    }
}
