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
                cfg.CreateMap<Tweetinvi.Models.ITweet, Tweet>()
                .ForMember(dest => dest.TweetUrl, opt => opt.MapFrom(src => src.Url))
                .ForMember(dest => dest.TweetCreatedBy, opt => opt.MapFrom(src => src.CreatedBy.Name))
                .ForMember(dest => dest.TweetCreatedByUrl, opt => opt.MapFrom(src => src.CreatedBy.Url))
                .ForMember(dest => dest.TweetCreatedOn, opt => opt.MapFrom(src => src.CreatedAt.DateTime))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdStr))
                .ForMember(dest => dest.HashTags, opt => opt.Ignore());
            });

            TweetMapper = configuration.CreateMapper();   
        }
    }
}