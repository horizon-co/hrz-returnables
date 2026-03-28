using System.Diagnostics.CodeAnalysis;

namespace Horizon.Returnables;

/// <summary>
/// Represents the outcome of an operation that does not return a value.
/// Use for commands, side-effects, or any void-returning operation where you want
/// success/failure to be explicit without throwing exceptions.
/// <para>For operations that return data, use <c>Result&lt;TValue&gt;</c> instead.</para>
/// </summary>
/// <remarks>
/// <para><b>Successful operation:</b></para>
/// <code>
/// public Result SaveUser(User user)
/// {
///     repository.Save(user);
///     return Result.Success;
/// }
/// </code>
///
/// <para><b>Failed operation:</b></para>
/// <code>
/// public Result DeleteUser(int id)
/// {
///     if (!repository.Exists(id))
///     {
///         return Result.Fail("User not found");
///     }
///
///     repository.Delete(id);
///     return Result.Success;
/// }
/// </code>
///
/// <para><b>Wrapping a try/catch:</b></para>
/// <code>
/// Result result = Result.Try(() =&gt; gateway.Charge(payment));
/// </code>
///
/// <para><b>Conditional factory:</b></para>
/// <code>
/// Result result = Result.FailIf(age &lt; 18, "Must be at least 18 years old");
/// </code>
///
/// <para>This is a <see langword="struct"/> — stack-allocated with no heap overhead on the success path.</para>
/// </remarks>
public readonly struct Result
{
    /// <summary>
    /// A pre-built failure with a generic "Process failed" message.
    /// Prefer <see cref="Fail(string?)"/> or <c>Fail&lt;TError&gt;(TError)</c> for descriptive errors.
    /// </summary>
    public static readonly Result Failure;

    /// <summary>
    /// A pre-built success result. Prefer this over <c>new Result()</c> for clarity.
    /// </summary>
    public static readonly Result Success;

    /// <summary>
    /// The exception that caused the failure, or <see langword="null"/> on success.
    /// </summary>
    public Exception? Error { get; }

    /// <summary>
    /// <see langword="true"/> when the operation failed.
    /// After checking this, the compiler knows <see cref="Error"/> is non-null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    public bool Failed => Error is not null;

    /// <summary>
    /// <see langword="true"/> when the operation succeeded.
    /// After checking this, the compiler knows <see cref="Error"/> is null.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Succeeded => Error is null;

    static Result()
    {
        Failure = Fail("Process failed");
        Success = new();
    }

    /// <summary>
    /// Initializes a successful result. Prefer <see cref="Success"/> for readability.
    /// </summary>
    public Result()
    {
        Error = default;
    }

    private Result(Exception error)
    {
        Error = error;
    }

    /// <summary>
    /// Creates a <c>Result&lt;TValue&gt;</c> from <paramref name="value"/>.
    /// Delegates to <c>Result&lt;TValue&gt;.From(TValue?)</c>.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="value">The value to wrap.</param>
    /// <returns>A successful <c>Result&lt;TValue&gt;</c> or a failure if <paramref name="value"/> is an <see cref="Exception"/>.</returns>
    public static Result<TValue> From<TValue>(TValue? value)
        => Result<TValue>.From(value);

    /// <summary>
    /// Creates a failure from a typed exception, preserving the full exception context
    /// (stack trace, inner exceptions, custom properties).
    /// </summary>
    /// <typeparam name="TError">The exception type.</typeparam>
    /// <param name="error">The exception. Must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="error"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// catch (TimeoutException ex)
    /// {
    ///     return Result.Fail(ex);
    /// }
    /// </code>
    /// </example>
    public static Result Fail<TError>(TError? error) where TError : Exception
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    /// <summary>
    /// Creates a failure from a plain error message.
    /// When <paramref name="message"/> is null/empty/whitespace, returns <see cref="Failure"/>.
    /// </summary>
    /// <param name="message">A human-readable error description.</param>
    /// <example>
    /// <code>
    /// return Result.Fail("Input validation failed: expected a positive number");
    /// </code>
    /// </example>
    public static Result Fail(string? message)
    {
        return string.IsNullOrWhiteSpace(message)
            ? Failure
            : new(new Exception(message));
    }

    /// <summary>
    /// Creates a failure with the default "Process failed" message.
    /// Equivalent to returning <see cref="Failure"/>.
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
    /// <example>
    /// <code>
    /// SaveUser(user).Switch(
    ///     onSuccess: () =&gt; logger.LogInformation("Saved"),
    ///     onFailure: error =&gt; logger.LogError(error, "Save failed"));
    /// </code>
    /// </example>
    public void Switch(Action? onSuccess, Action<Exception>? onFailure)
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
    /// Executes <paramref name="action"/> as a side-effect on success, then returns the same result.
    /// Does nothing on failure. Useful for logging or telemetry without breaking a fluent chain.
    /// </summary>
    /// <param name="action">The action to execute on success.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="action"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// DeleteUser(id)
    ///     .OnSuccess(() =&gt; logger.LogInformation("User {Id} deleted", id))
    ///     .OnFailure(error =&gt; logger.LogError(error, "Delete failed"));
    /// </code>
    /// </example>
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
    /// Executes <paramref name="action"/> as a side-effect on failure, then returns the same result.
    /// Does nothing on success. Useful for logging errors without breaking a fluent chain.
    /// </summary>
    /// <param name="action">The action to execute with the error.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="action"/> is <see langword="null"/>.</exception>
    public Result OnFailure(Action<Exception>? action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (Failed)
        {
            action(Error);
        }

        return this;
    }

    /// <summary>
    /// Wraps a synchronous action in a try/catch. Returns <see cref="Success"/> if the action
    /// completes, or a failure containing the caught exception.
    /// </summary>
    /// <param name="action">The operation to attempt.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="action"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// Result result = Result.Try(() =&gt; fileSystem.Delete(path));
    /// </code>
    /// </example>
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
            return Result.Fail(ex);
        }
    }

    /// <summary>
    /// Wraps an async operation in a try/catch. Returns <see cref="Success"/> if the task
    /// completes, or a failure containing the caught exception.
    /// </summary>
    /// <param name="func">The async operation to attempt.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="func"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// Result result = await Result.TryAsync(() =&gt; httpClient.PostAsync(url, content));
    /// </code>
    /// </example>
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
            return Result.Fail(ex);
        }
    }

    /// <summary>
    /// Returns <see cref="Success"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or a failure with <paramref name="error"/> when <see langword="false"/>.
    /// Inverse of <c>FailIf&lt;TError&gt;(bool, TError)</c>.
    /// </summary>
    /// <typeparam name="TError">The exception type.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="error">The exception to use on failure.</param>
    /// <example>
    /// <code>
    /// Result result = Result.SuccessIf(user.IsActive, new UnauthorizedAccessException("Account disabled"));
    /// </code>
    /// </example>
    public static Result SuccessIf<TError>(bool condition, TError? error) where TError : Exception
    {
        return FailIf(!condition, error);
    }

    /// <summary>
    /// Returns <see cref="Success"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or a failure with <paramref name="errorMessage"/> when <see langword="false"/>.
    /// Inverse of <see cref="FailIf(bool, string?)"/>.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="errorMessage">The error message on failure.</param>
    /// <example>
    /// <code>
    /// Result result = Result.SuccessIf(age &gt;= 18, "Must be at least 18 years old");
    /// </code>
    /// </example>
    public static Result SuccessIf(bool condition, string? errorMessage)
    {
        return FailIf(!condition, errorMessage);
    }

    /// <summary>
    /// Returns a failure with <paramref name="error"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or <see cref="Success"/> when <see langword="false"/>.
    /// </summary>
    /// <typeparam name="TError">The exception type.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="error">The exception to use on failure.</param>
    public static Result FailIf<TError>(bool condition, TError? error) where TError : Exception
        => condition ? Fail(error) : Result.Success;

    /// <summary>
    /// Returns a failure with <paramref name="errorMessage"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or <see cref="Success"/> when <see langword="false"/>.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="errorMessage">The error message on failure.</param>
    /// <example>
    /// <code>
    /// Result result = Result.FailIf(string.IsNullOrEmpty(name), "Name is required");
    /// </code>
    /// </example>
    public static Result FailIf(bool condition, string? errorMessage)
        => condition ? Fail(errorMessage) : Result.Success;

    /// <summary>
    /// Converts a <see cref="Result"/> to its underlying <see cref="Exception"/>,
    /// returning <see langword="null"/> on success.
    /// </summary>
    public static implicit operator Exception?(Result result)
    {
        return result.Error;
    }

    /// <summary>
    /// Converts an <see cref="Exception"/> into a failed <see cref="Result"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// Result result = new InvalidOperationException("Duplicate entry");
    /// </code>
    /// </example>
    public static implicit operator Result(Exception? error)
    {
        return Fail(error);
    }
}