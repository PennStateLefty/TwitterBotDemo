using System;

namespace TwitterBot.Framework.Contracts.ServiceBus
{
	public interface IServiceBusContext
	{
		public String ConnectionString { get; set; }
		public String QueueName { get; set; }
		public String SessionId { get; set; }
		public int MaxConcurrentMessagesToBeRetrieved { get; set; }
		public TimeSpan OperationTimeout { get; set; }
	}
}
