namespace ForeignExchangeRates.WebAPI.Models;

public class UserPostModel
{
	public required string Username { get; set; }
	public required string Password { get; set; }
}

public class UserDto
{
	public required string Username { get; set; }
}

public class UserLoginModel
{
	public required string Username { get; set; }
	public required string Password { get; set; }
}
