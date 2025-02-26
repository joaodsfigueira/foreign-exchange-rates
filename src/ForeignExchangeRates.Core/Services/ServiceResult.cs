namespace ForeignExchangeRates.Core.Services;

public abstract record ServiceResult<T>(bool IsSuccess)
{
	public bool IsSuccess { get; } = IsSuccess;
	public static SuccessResult<T> Success(T value) => new(value);
	public static ConflictResult<T> Conflict() => new();
	public static NotFoundResult<T> NotFound() => new();
	public static ValidationFailedResult<T> ValidationFailed(IDictionary<string, string[]>? errors) => new(errors);
}

public record SuccessResult<T>(T ResultObject) : ServiceResult<T>(true)
{
	public T ResultObject { get; } = ResultObject;
}
public record ConflictResult<T>() : ServiceResult<T>(false);
public record NotFoundResult<T>() : ServiceResult<T>(false);
public record ValidationFailedResult<T>(IDictionary<string, string[]>? Errors) : ServiceResult<T>(false)
{
	public IDictionary<string, string[]>? Errors { get; } = Errors;
}
