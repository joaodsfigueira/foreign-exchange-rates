namespace ForeignExchangeRates.Core.Interfaces;

public interface IEventSourcingProvider
{
    Task SaveEventAsync(EventMessage eventMessage);
}

public class NullEventSourcingProvider : IEventSourcingProvider
{
	public Task SaveEventAsync(EventMessage eventMessage)
	{
        return Task.CompletedTask;
	}
}

public class EventMessage 
{
    public EventType EventType { get; set; }
    public string? Body { get; set; }
}

public enum EventType
{
    Created, 
    Updated, 
    Deleted
}
