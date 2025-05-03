using System.Diagnostics.CodeAnalysis;

namespace Bookify.ShareKernel.Result;

public class Result
{
    public Result(bool isSuccess, Error.Error error)
    {
        if (isSuccess && error != ShareKernel.Error.Error.None) throw new InvalidOperationException();
        if (!isSuccess && error == ShareKernel.Error.Error.None) throw new InvalidOperationException();
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error.Error Error { get; }
    public static Result Success() => new(true, ShareKernel.Error.Error.None);
    public static Result Failure(Error.Error error) => new(false, error);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, ShareKernel.Error.Error.None);
    public static Result<TValue> Failure<TValue>(Error.Error error) => new(default, false, error);
    public static Result<TValue> Create<TValue>(TValue? value) => value is null ? Failure<TValue>(ShareKernel.Error.Error.NullValue) : Success(value);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;
    public Result(TValue? value, bool isSuccess, Error.Error error)
        :base(isSuccess, error)
    {
        _value = value;
    }
    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");
    //public static implicit operator Result<TValue>(TValue? value) => Create(value);
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(ShareKernel.Error.Error.NullValue);
    public static Result<TValue> ValidationFailure(Error.Error error) =>
        new(default, false, error);
}