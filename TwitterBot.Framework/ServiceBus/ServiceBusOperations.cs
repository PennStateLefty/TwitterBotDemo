using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using TwitterBot.Framework.Contracts.ServiceBus;

namespace TwitterBot.Framework.ServiceBus
{
	public class ServiceBusOperations : IServiceBusOperations
	{
		private readonly IServiceBusContext _context;
		private readonly IMessageSender _messageSender;
		private readonly ISessionClient _sessionClient;

		public ServiceBusOperations(IServiceBusContext context)
		{
			_context = context;
			_messageSender = new MessageSender(context.ConnectionString, context.QueueName);
			_sessionClient = new SessionClient(context.ConnectionString, context.QueueName);
		}

		public async Task<List<Message>> ReceiveMessagesAsync()
		{
			var messages = new List<Message>();
			IMessageSession session = await _sessionClient.AcceptMessageSessionAsync(_context.SessionId);

			if (session == null)
			{
				return messages;
			}

			for (int i = 0; i < _context.MaxConcurrentMessagesToBeRetrieved; i++)
			{
				Message message = await session.ReceiveAsync(_context.OperationTimeout);

				if (message == null)
				{
					break;
				}

				messages.Add(message);
				await session.CompleteAsync(message.SystemProperties.LockToken);
			}

			await session.CloseAsync();
			return messages;
		}

		public async Task SendMessagesAsync(string id, string message)
		{
			await _messageSender.SendAsync(new Message(Encoding.UTF8.GetBytes(message))
			{
				MessageId = id,
				SessionId = _context.SessionId
			});
		}
	}
}
