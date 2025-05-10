using Bookify.ShareKernel.Utilities;

namespace Bookify.API.Extensions;

public static class ResultExtensions
{
    // Bookify.ShareKernel.Result không xử lý được hành vi (behavior) — chỉ đại diện cho dữ liệu trạng thái.
    // ResultExtensions.Match() là cách để định nghĩa hành vi tương ứng với trạng thái thành công hoặc thất bại, và giúp tránh boilerplate code.
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<Result, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result);
    }

    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Result<TIn>, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
    }
    
    public static async Task<TOut> MatchAsync<TOut>(
        this Result result,
        Func<Task<TOut>> onSuccess,
        Func<Result, Task<TOut>> onFailure)
    {
        return result.IsSuccess
            ? await onSuccess()
            : await onFailure(result);
    }

    public static async Task<TOut> MatchAsync<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, Task<TOut>> onSuccess,
        Func<Result<TIn>, Task<TOut>> onFailure)
    {
        return result.IsSuccess
            ? await onSuccess(result.Value)
            : await onFailure(result);
    }
}