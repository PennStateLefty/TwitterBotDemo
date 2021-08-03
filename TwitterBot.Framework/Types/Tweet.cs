using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TwitterBot.Framework.Types
{
    public class Tweet : BaseType
    {
        [JsonProperty(PropertyName = "txt")]
        public String Text { get; set; }
        [JsonProperty(PropertyName = "ftxt")]
        public String FullText { get; set; }
        [JsonProperty(PropertyName = "turl")]
        public String TweetUrl { get; set; }
        [JsonProperty(PropertyName = "rcnt")]
        public int RetweetCount { get; set; }
        [JsonProperty(PropertyName = "fcnt")]
        public int FavoriteCount { get; set; }
        [JsonProperty(PropertyName = "itd")]
        public bool IsTweetDestroyed { get; set; }
        [JsonProperty(PropertyName = "tcb")]
        public String TweetCreatedBy { get; set; }
        [JsonProperty(PropertyName = "tcbu")]
        public String TweetCreatedByUrl { get; set; }
        [JsonProperty(PropertyName = "tco")]
        public DateTime TweetCreatedOn { get; set; }
        [JsonProperty(PropertyName = "hts")]
        public ICollection<Hashtag> HashTags { get; set; }
    }
}