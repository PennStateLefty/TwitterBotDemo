using System;
using Newtonsoft.Json;

namespace TwitterBot.Framework.Types
{
    public class BaseType
    {
        [JsonProperty(PropertyName = "id")]
        public String Id { get; set; }
        //These JsonProperty annotations tell the JSON serializer to how to represent shorter names in JSON and still map to full objects
        [JsonProperty(PropertyName = "con")]
        public DateTime CreatedOn { get; set; }
        [JsonProperty(PropertyName = "cby")]
        public String CreatedBy { get; set; }
        [JsonProperty(PropertyName = "mon")]
        public DateTime ModifiedOn { get; set; }
        [JsonProperty(PropertyName = "mby")]
        public String ModifiedBy { get; set; }
        [JsonProperty(PropertyName = "isd")]
        public bool IsDeleted { get; set; }
    }
}