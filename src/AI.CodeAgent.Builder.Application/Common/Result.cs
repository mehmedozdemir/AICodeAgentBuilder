namespace AI.CodeAgent.Builder.Application.Common;

/// <summary>
/// Result wrapper for application operations.
/// Provides a consistent way to return success or failure with error messages.
/// </summary>
/// <typeparam name="T">The type of the value returned on success</typeparam>
public sealed class Result<T>
{
    private Result(bool isSuccess, T? value, IEnumerable<string>? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors?.ToList() ?? new List<string>();
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public IReadOnlyList<string> Errors { get; }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(string error) => new(false, default, new[] { error });
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors);
}

/// <summary>
/// Result wrapper for operations without return value.
/// </summary>
public sealed class Result
{
    private Result(bool isSuccess, IEnumerable<string>? errors)
    {
        IsSuccess = isSuccess;
        Errors = errors?.ToList() ?? new List<string>();
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<string> Errors { get; }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, new[] { error });
    public static Result Failure(IEnumerable<string> errors) => new(false, errors);
}
