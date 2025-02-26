using Azure.Messaging.ServiceBus;
using ForeignExchangeRates.Core.Interfaces;
using System.Text.Json;

namespace ForeignExchangeRates.Infrastructure.Providers;

public class AzureServiceBusProvider : IEventSourcingProvider
{
	ServiceBusSender _serviceBusSender;

	public AzureServiceBusProvider(ServiceBusClient serviceBusClient, string queueName)
	{
		_serviceBusSender = serviceBusClient.CreateSender(queueName);
	}
	
	public async Task SaveEventAsync(EventMessage eventMessage)
	{
		await _serviceBusSender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(eventMessage)));
	}
}
