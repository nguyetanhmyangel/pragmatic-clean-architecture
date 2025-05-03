using Bookify.ShareKernel.Error;

namespace Bookify.ShareKernel.Exceptions;
// public sealed record ValidationError(string PropertyName, string ErrorMessage);
public sealed record ValidationError : Error.Error
{
    public ValidationError(Error.Error[] errors)
        : base(
            "Validation.General",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    public Error.Error[] Errors { get; }

    public static ValidationError FromResults(IEnumerable<Result.Result> results) =>
        new(results.Where(r => r.IsFailure).Select(r => r.Error).ToArray());
}