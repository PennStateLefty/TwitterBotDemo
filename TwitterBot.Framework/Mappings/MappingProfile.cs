using System;
using AutoMapper;

using TwitterBot.Framework.Types;

namespace TwitterBot.Framework.Mappings
{
    public class MappingProfile
    {
        public static readonly IMapper TweetMapper;
        static MappingProfile()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Tweetinvi.Models.ITweet, Tweet>();
            });

            TweetMapper = configuration.CreateMapper();   
        }
    }
}