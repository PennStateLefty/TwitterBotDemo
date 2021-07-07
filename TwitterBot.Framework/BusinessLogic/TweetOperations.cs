using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

using TwitterBot.Framework.Contracts;
using TwitterBot.Framework.Mappings;
using TwitterBot.Framework.Types;

namespace TwitterBot.Framework.BusinessLogic
{
    public class TweetOperations : ITweetOperations
    {
        private readonly String _consumerKey;
        private readonly String _consumerSecret;
        private readonly String _bearerToken;

        public TweetOperations(String consumerKey, String consumerSecret, String bearerToken)
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
            _bearerToken = bearerToken;
        }

        public async Task<Tweet> GetPopularTweetsByHashtagAsync(Hashtag tag)
        {
            try 
            {
                TwitterClient twitterClient = new TwitterClient(_consumerKey, _consumerSecret, _bearerToken);

                //Setup search parameters
                ISearchTweetsParameters searchParameter = new SearchTweetsParameters(tag.Text);
                searchParameter.SearchType = SearchResultType.Popular;
                searchParameter.PageSize = 1;

                var tweetSearch = await twitterClient.Search.SearchTweetsAsync(searchParameter);

                if(!tweetSearch.Any())
                {
                    return null;
                }
                
                return MappingProfile.TweetMapper.Map<Tweet>(tweetSearch.FirstOrDefault());
            }
            catch (Exception ex)
            {
                //Log Exception Here
                System.Console.WriteLine(ex.Message);
            }

            return null;
        }
    }
}