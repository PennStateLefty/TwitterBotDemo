using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace TwitterBot.Framework.Types
{
	public class User : BaseType
	{
		[JsonProperty(PropertyName = "uid")]
		public String UserId { get; set; }
		[JsonProperty(PropertyName = "hts")]
		public ICollection<Hashtag> HashTags { get; set; }
	}
}
