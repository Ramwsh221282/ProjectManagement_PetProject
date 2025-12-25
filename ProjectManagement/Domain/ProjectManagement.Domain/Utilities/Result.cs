namespace ProjectManagement.Domain.Utilities;

public static class Result
{
    public static Result<T, Error> Failure<T>(Error error)
    {
        return Result<T, Error>.Error(error);
    }
    
    public static Result<T, U> Success<T, U>(T onSuccess)
    {
        return Result<T, U>.Success(onSuccess);
    }
    
    public static Result<T, U> Failure<T, U>(U onError)
    {
        return Result<T, U>.Error(onError);
    }

    public static Result<T, U> Continue<T, U>(this Result<T, U> result, Func<Result<T,U>> next)
    {
        return result.IsFailure ? result : next();
    }
    
    public static Result<T, U> Continue<T, U>(this Result<T, U> result, Func<Result<T, U>, Result<T,U>> next)
    {
        return result.IsFailure ? result : next(result);
    }
    
    public static Result<T, U> Map<T, U>(this Result<T, U> result, Func<T, Result<T, U>> next)
    {
        return result.IsFailure ? result : next(result.OnSuccess);
    }
}

public class Result<T, U>
{
    /// <summary>
    /// Успех
    /// </summary>
    private readonly T? _onSuccess;
    
    /// <summary>
    /// Ошибка
    /// </summary>
    private readonly U? _onError;
    
    /// <summary>
    /// Получение успеха
    /// </summary>
    /// <exception cref="InvalidOperationException">При доступе к успеху, в случае ошибки</exception>
    public T OnSuccess => _onSuccess ?? throw new InvalidOperationException("Result is failure.");
    
    /// <summary>
    /// Получение ошибки
    /// </summary>
    /// <exception cref="InvalidOperationException">При доступе к ошибке, в случае успеха</exception>
    public U OnError => _onError ?? throw new InvalidOperationException("Result is success.");
    
    /// <summary>
    /// Признак успеха
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// Признак ошибки
    /// </summary>
    public bool IsFailure => !IsSuccess;
    
    /// <summary>
    /// Успех
    /// </summary>
    /// <param name="onSuccess">Что отдавать при успехе</param>
    protected Result(T onSuccess)
    {
        _onSuccess = onSuccess;
        IsSuccess = true;
    }
    
    /// <summary>
    /// Ошибка
    /// </summary>
    /// <param name="onError">Что отдавать при ошибке</param>
    protected Result(U onError)
    {
        _onError = onError;
        IsSuccess = false;
    }

    public static Result<T, U> Success(T value) => new(value);
    public static Result<T, U> Error(U value) => new(value);
}