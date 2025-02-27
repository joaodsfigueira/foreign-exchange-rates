namespace ForeignExchangeRates.Core.Entities;

public class User
{
	public required Guid Id { get; set; }
	public required string Username { get; set; }
	public required string PasswordHash { get; set; }
}
