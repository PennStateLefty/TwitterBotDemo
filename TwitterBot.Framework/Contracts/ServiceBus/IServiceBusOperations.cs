using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TwitterBot.Framework.Contracts.ServiceBus
{
	public interface IServiceBusOperations
	{
		public Task SendMessagesAsync(String id, String message);
		public Task<List<Message>> ReceiveMessagesAsync();
	}
}
