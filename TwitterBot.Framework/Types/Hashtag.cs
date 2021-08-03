using Newtonsoft.Json;
using System;

namespace TwitterBot.Framework.Types
{
    public class Hashtag : BaseType
    {
        [JsonProperty(PropertyName = "txt")]
        public String Text { get; set; }
        [JsonProperty(PropertyName = "lsdt")]
        public DateTime LastSyncDateTime { get; set; }
        [JsonProperty(PropertyName = "iciq")]
        public bool IsCurrentlyInQueue { get; set; }
    }
}