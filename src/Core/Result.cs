using System.Diagnostics.CodeAnalysis;

namespace Hrz.Returnables;

/// <summary>
/// Represents the outcome of an operation that does not return a value.
/// </summary>
public readonly struct Result
{
    /// <summary>
    /// A pre-built failure with a generic "Process failed" message.
    /// </summary>
    public static readonly Result Failure;

    /// <summary>
    /// A pre-built success result.
    /// </summary>
    public static readonly Result Success;

    /// <summary>
    /// The error that caused the failure, or <see langword="null"/> on success.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// <see langword="true"/> when the operation failed.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool Failed => Error is not null;

    /// <summary>
    /// <see langword="true"/> when the operation succeeded.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Succeeded => Error is null;

    static Result()
    {
        Failure = new(Error.Create("FAILURE", "Process failed"));
        Success = new();
    }

    /// <summary>
    /// Initializes a successful result.
    /// </summary>
    public Result()
    {
        Error = default;
    }

    private Result(Error error)
    {
        Error = error;
    }

    /// <summary>
    /// Creates a <c>Result&lt;TValue&gt;</c> from <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value to wrap.</param>
    public static Result<TValue> From<TValue>(TValue? value)
        => Result<TValue>.From(value);

    /// <summary>
    /// Creates a failure from a typed error.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="error">The error. Must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="error"/> is <see langword="null"/>.</exception>
    public static Result Fail<TError>(TError? error) where TError : Error
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    /// <summary>
    /// Creates a failure with the specified code and message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public static Result Fail(string? code, string? message)
    {
        return new(Error.Create(code, message));
    }

    /// <summary>
    /// Creates a failure with the default "Process failed" message.
    /// </summary>
    public static Result Fail()
    {
        return Failure;
    }

    /// <summary>
    /// Executes one of two actions depending on the outcome.
    /// </summary>
    /// <param name="onSuccess">Invoked when the operation succeeded.</param>
    /// <param name="onFailure">Invoked with <see cref="Error"/> when the operation failed.</param>
    /// <exception cref="ArgumentNullException">When either delegate is <see langword="null"/>.</exception>
    public void Switch(Action? onSuccess, Action<Error>? onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        if (Succeeded)
        {
            onSuccess();
        }
        else
        {
            onFailure(Error);
        }
    }

    /// <summary>
    /// Executes <paramref name="action"/> on success, then returns the same result.
    /// </summary>
    /// <param name="action">The action to execute on success.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="action"/> is <see langword="null"/>.</exception>
    public Result OnSuccess(Action? action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (Succeeded)
        {
            action();
        }

        return this;
    }

    /// <summary>
    /// Executes <paramref name="action"/> on failure, then returns the same result.
    /// </summary>
    /// <param name="action">The action to execute with the error.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="action"/> is <see langword="null"/>.</exception>
    public Result OnFailure(Action<Error>? action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (Failed)
        {
            action(Error);
        }

        return this;
    }

    /// <summary>
    /// Wraps a synchronous action in a try/catch.
    /// </summary>
    /// <param name="action">The operation to attempt.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="action"/> is <see langword="null"/>.</exception>
    public static Result Try(Action? action)
    {
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            action();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return new(Error.FromException(ex));
        }
    }

    /// <summary>
    /// Wraps an async operation in a try/catch.
    /// </summary>
    /// <param name="func">The async operation to attempt.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="func"/> is <see langword="null"/>.</exception>
    public static async Task<Result> TryAsync(Func<Task>? func)
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            await func.Invoke();
            return Result.Success;
        }
        catch (Exception ex)
        {
            return new(Error.FromException(ex));
        }
    }

    /// <summary>
    /// Returns <see cref="Success"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or a failure with <paramref name="error"/> when <see langword="false"/>.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="error">The error to use on failure.</param>
    public static Result SuccessIf<TError>(bool condition, TError? error) where TError : Error
    {
        return FailIf(!condition, error);
    }

    /// <summary>
    /// Returns <see cref="Success"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or a failure with the specified code and message when <see langword="false"/>.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="code">The error code on failure.</param>
    /// <param name="message">The error message on failure.</param>
    public static Result SuccessIf(bool condition, string? code, string? message)
    {
        return FailIf(!condition, code, message);
    }

    /// <summary>
    /// Returns a failure with <paramref name="error"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or <see cref="Success"/> when <see langword="false"/>.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="error">The error to use on failure.</param>
    public static Result FailIf<TError>(bool condition, TError? error) where TError : Error
        => condition ? Fail(error) : Result.Success;

    /// <summary>
    /// Returns a failure with the specified code and message when <paramref name="condition"/> is <see langword="true"/>,
    /// or <see cref="Success"/> when <see langword="false"/>.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="code">The error code on failure.</param>
    /// <param name="message">The error message on failure.</param>
    public static Result FailIf(bool condition, string? code, string? message)
        => condition ? Fail(code, message) : Result.Success;

    /// <summary>
    /// Converts a <see cref="Result"/> to its underlying <see cref="Error"/>.
    /// </summary>
    public static implicit operator Error?(Result result)
    {
        return result.Error;
    }

    /// <summary>
    /// Converts an <see cref="Error"/> into a failed <see cref="Result"/>.
    /// </summary>
    public static implicit operator Result(Error? error)
    {
        return Fail(error);
    }
}

/// <summary>
/// Represents the outcome of an operation that returns a value of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The type of the value produced on success.</typeparam>
public readonly struct Result<TValue>
{
    /// <summary>
    /// A pre-built failure with a generic "Process failed" message.
    /// </summary>
    public static readonly Result<TValue> Failure;

    /// <summary>
    /// The value produced on success, or <see langword="default"/> on failure.
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// The error that caused the failure, or <see langword="null"/> on success.
    /// </summary>
    public Error? Error { get; }

    /// <summary>
    /// <see langword="true"/> when the operation succeeded.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Succeeded => Error is null && Value is not null;

    /// <summary>
    /// <see langword="true"/> when the operation failed.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool Failed => Error is not null;

    static Result()
    {
        Failure = Fail("FAILURE", "Process failed");
    }

    private Result(TValue value)
    {
        Value = value;
        Error = default;
    }

    private Result(Error error)
    {
        Value = default;
        Error = error;
    }

    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> of a different type from <paramref name="value"/>.
    /// </summary>
    /// <typeparam name="TNewValue">The target value type.</typeparam>
    /// <param name="value">The value to wrap.</param>
    public static Result<TNewValue> From<TNewValue>(TNewValue? value)
        => Result<TNewValue>.From(value);

    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> from <paramref name="value"/>.
    /// If <paramref name="value"/> is an <see cref="Error"/>, returns a failure wrapping it.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="value"/> is <see langword="null"/> and not an <see cref="Error"/>.</exception>
    public static Result<TValue> From(TValue? value)
    {
        if (value is Error error)
        {
            return Fail(error);
        }

        ArgumentNullException.ThrowIfNull(value);
        return new(value);
    }

    /// <summary>
    /// Creates a failure from a typed error.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="error">The error. Must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="error"/> is <see langword="null"/>.</exception>
    public static Result<TValue> Fail<TError>(TError? error) where TError : Error
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    /// <summary>
    /// Creates a failure with the specified code and message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public static Result<TValue> Fail(string? code, string? message)
    {
        return new(Error.Create(code, message));
    }

    /// <summary>
    /// Creates a failure with the default "Process failed" message.
    /// </summary>
    public static Result<TValue> Fail()
    {
        return Failure;
    }

    /// <summary>
    /// Executes one of two actions depending on the outcome.
    /// </summary>
    /// <param name="onSuccess">Invoked with <see cref="Value"/> on success.</param>
    /// <param name="onFailure">Invoked with <see cref="Error"/> on failure.</param>
    /// <exception cref="ArgumentNullException">When either delegate is <see langword="null"/>.</exception>
    public void Switch(Action<TValue> onSuccess, Action<Error> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        if (Succeeded)
        {
            onSuccess(Value);
        }
        else
        {
            onFailure(Error);
        }
    }

    /// <summary>
    /// Executes <paramref name="action"/> on success, then returns the same result.
    /// </summary>
    /// <param name="action">The action to execute with the success value.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="action"/> is <see langword="null"/>.</exception>
    public Result<TValue> OnSuccess(Action<TValue> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (Succeeded)
        {
            action(Value);
        }

        return this;
    }

    /// <summary>
    /// Executes <paramref name="action"/> on failure, then returns the same result.
    /// </summary>
    /// <param name="action">The action to execute with the error.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="action"/> is <see langword="null"/>.</exception>
    public Result<TValue> OnFailure(Action<Error> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (Failed)
        {
            action(Error);
        }

        return this;
    }

    /// <summary>
    /// Wraps a synchronous delegate in a try/catch.
    /// </summary>
    /// <param name="func">The operation to attempt.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="func"/> is <see langword="null"/>.</exception>
    public static Result<TValue> Try(Func<TValue> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            return new(func());
        }
        catch (Exception ex)
        {
            return new(Error.FromException(ex));
        }
    }

    /// <summary>
    /// Wraps an async delegate in a try/catch.
    /// </summary>
    /// <param name="func">The async operation to attempt.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="func"/> is <see langword="null"/>.</exception>
    public static async Task<Result<TValue>> TryAsync(Func<Task<TValue>> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            return new(await func());
        }
        catch (Exception ex)
        {
            return new(Error.FromException(ex));
        }
    }

    /// <summary>
    /// Returns a success when <paramref name="condition"/> is <see langword="true"/>,
    /// or a failure with <paramref name="error"/> when <see langword="false"/>.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="value">The value to return on success.</param>
    /// <param name="error">The error to use on failure.</param>
    public static Result<TValue> SuccessIf<TError>(bool condition, TValue? value, TError? error) where TError : Error
    {
        return FailIf(!condition, value, error);
    }

    /// <summary>
    /// Returns a success when <paramref name="condition"/> is <see langword="true"/>,
    /// or a failure with the specified code and message when <see langword="false"/>.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="value">The value to return on success.</param>
    /// <param name="code">The error code on failure.</param>
    /// <param name="message">The error message on failure.</param>
    public static Result<TValue> SuccessIf(bool condition, TValue? value, string? code, string? message)
    {
        return FailIf(!condition, value, code, message);
    }

    /// <summary>
    /// Returns a failure with <paramref name="error"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or a success with <paramref name="value"/> when <see langword="false"/>.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="value">The value to return on success.</param>
    /// <param name="error">The error to use on failure.</param>
    public static Result<TValue> FailIf<TError>(bool condition, TValue? value, TError? error) where TError : Error
        => condition ? Fail(error) : From(value);

    /// <summary>
    /// Returns a failure with the specified code and message when <paramref name="condition"/> is <see langword="true"/>,
    /// or a success with <paramref name="value"/> when <see langword="false"/>.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="value">The value to return on success.</param>
    /// <param name="code">The error code on failure.</param>
    /// <param name="message">The error message on failure.</param>
    public static Result<TValue> FailIf(bool condition, TValue? value, string? code, string? message)
        => condition ? Fail(code, message) : From(value);

    /// <summary>
    /// Converts a <see cref="Result{TValue}"/> to a void <see cref="Result"/>.
    /// </summary>
    public static implicit operator Result(Result<TValue> result)
        => result.Failed ? result.Error : Result.Success;

    /// <summary>
    /// Converts an <see cref="Error"/> into a failed <see cref="Result{TValue}"/>.
    /// </summary>
    public static implicit operator Result<TValue>(Error error)
        => Fail(error);

    /// <summary>
    /// Unwraps <see cref="Value"/> from the result.
    /// </summary>
    public static implicit operator TValue?(Result<TValue> result)
        => result.Value;

    /// <summary>
    /// Extracts <see cref="Error"/> from the result.
    /// </summary>
    public static implicit operator Error?(Result<TValue> result)
        => result.Error;
}
