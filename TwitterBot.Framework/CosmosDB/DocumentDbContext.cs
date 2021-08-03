using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using TwitterBot.Framework.Contracts.Data;
using TwitterBot.Framework.Types;

namespace TwitterBot.Framework.CosmosDB
{
	public class DocumentDbContext : IDocumentDbContext
	{
		private IDocumentClient _documentClient;
		private IList<IDocumentDbEntity> _documentDbEntities;

		public string DatabaseId { get; set; }
		public string EndpointUri { get; set; }
		public string AuthKey { get; set; }

		public IDocumentClient DocumentClient
		{
			get
			{
				if (_documentClient == null)
				{
					_documentClient = GetDocumentClient();
				}

				return _documentClient;
			}
		}
		public ICollection<IDocumentDbEntity> EntityCollection
		{
			get
			{
				if (_documentDbEntities == null)
				{
					_documentDbEntities = GetDocumentEntities();
				}

				return _documentDbEntities;
			}
		}

		public async Task CreateDatabaseAndCollectionsAsync()
		{
			await CreateDatabaseAsync(DatabaseId);
			foreach (IDocumentDbEntity entity in EntityCollection)
			{
				await CreateCollectionsAsync(DatabaseId, entity.Name);
			}
		}

		private IDocumentClient GetDocumentClient()
		{
			var connectionPolicy = new ConnectionPolicy
			{
				ConnectionMode = ConnectionMode.Gateway,
				ConnectionProtocol = Protocol.Https,
				MaxConnectionLimit = 1000,
				RetryOptions = new RetryOptions
				{
					MaxRetryAttemptsOnThrottledRequests = 3,
					MaxRetryWaitTimeInSeconds = 30
				},
				EnableEndpointDiscovery = true,
				EnableReadRequestsFallback = true
			};

			connectionPolicy.PreferredLocations.Add(LocationNames.CentralUS);
			var client = new DocumentClient(new Uri(EndpointUri), AuthKey, connectionPolicy);

			return client;
		}

		private List<IDocumentDbEntity> GetDocumentEntities()
		{
			var entityCollection = new List<IDocumentDbEntity>()
			{
				new DocumentDbEntity { EntityType = typeof(Tweet), Name = "TweetCollection" },
				new DocumentDbEntity { EntityType = typeof(Hashtag), Name = "HashtagCollection" }
			};

			return entityCollection;
		}

		private async Task<Database> CreateDatabaseAsync(String databaseId)
		{
			var response = await DocumentClient.CreateDatabaseIfNotExistsAsync(new Database
			{
				Id = databaseId
			});

			return response.Resource;
		}

		private async Task<DocumentCollection> CreateCollectionsAsync(String databaseId, String collectionName)
		{
			var response = await DocumentClient.CreateDocumentCollectionIfNotExistsAsync(
				UriFactory.CreateDatabaseUri(databaseId),
				new DocumentCollection
				{
					Id = collectionName
				},
				new RequestOptions());

			return response.Resource;
		}
	}
}
