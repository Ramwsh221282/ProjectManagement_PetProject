namespace ProjectManagement.Domain.Utilities;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error OnError { get; }

    protected Result()
    {
        IsSuccess = true;
        OnError = Error.None();
    }

    protected Result(Error error)
    {
        IsSuccess = false;
        OnError = error;
    }

    public static Result Success() => new();

    public static Result Failure(Error error) => new(error);

    public static Result<T> Failure<T>(Error error)
        where T : notnull => Result<T>.Failure(error);

    public static Result<T> Success<T>(T value)
        where T : notnull => Result<T>.Success(value);

    public static implicit operator Result(Error error) => Failure(error);
}

public class Result<T> : Result
    where T : notnull
{
    /// <summary>
    /// Успех
    /// </summary>
    private readonly T? _onSuccess;

    /// <summary>
    /// Получение успеха
    /// </summary>
    /// <exception cref="InvalidOperationException">При доступе к успеху, в случае ошибки</exception>
    public T OnSuccess => _onSuccess ?? throw new InvalidOperationException("Result is failure.");

    /// <summary>
    /// Успех
    /// </summary>
    /// <param name="onSuccess">Что отдавать при успехе</param>
    protected Result(T onSuccess)
    {
        _onSuccess = onSuccess;
    }

    /// <summary>
    /// Ошибка
    /// </summary>
    /// <param name="onError">Что отдавать при ошибке</param>
    protected Result(Error onError)
        : base(onError) { }

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(Error error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(Error error) => Failure(error);
}
