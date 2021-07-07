using System;
using System.Threading.Tasks;

using TwitterBot.Framework.Types;

namespace TwitterBot.Framework.Contracts
{
    public interface ITweetOperations
    {
        public Task<Tweet> GetPopularTweetsByHashtagAsync (Hashtag tag);
    }
}