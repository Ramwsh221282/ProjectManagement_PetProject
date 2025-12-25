namespace ProjectManagement.Domain.Utilities;

public sealed class ErrorResult<T> : Result<T, Error>
{
    public ErrorResult(T onSuccess) : base(onSuccess)
    {
    }

    public ErrorResult(Error onError) : base(onError)
    {
    }
    
    public static implicit operator ErrorResult<T>(Error error) => new(error);
    public static implicit operator ErrorResult<T>(T value) => new(value);
}