using Polly;
using Polly.Extensions.Http;

namespace ForeignExchangeRates.WebAPI.Polly;

public static class RetryPolicyProvider
{
	public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
	{
		return HttpPolicyExtensions
			.HandleTransientHttpError()
			.WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
	}
}
