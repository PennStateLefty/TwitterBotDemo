using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;

namespace TwitterBot.Framework.Contracts.Data
{
	public interface IDocumentDbContext
	{
		String DatabaseId { get; set; }
		String EndpointUri { get; set; }
		String AuthKey { get; set; }
		IDocumentClient DocumentClient { get; }
		ICollection<IDocumentDbEntity> EntityCollection { get; }
		Task CreateDatabaseAndCollectionsAsync();
	}
}
