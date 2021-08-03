using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TwitterBot.Framework.Contracts.Data;

namespace TwitterBot.Framework.CosmosDB
{
	public class DocumentDbEntity : IDocumentDbEntity
	{
		public Type EntityType { get; set; }
		public string Name { get; set; }
	}
}
