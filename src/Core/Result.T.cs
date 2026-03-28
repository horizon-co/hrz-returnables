using System.Diagnostics.CodeAnalysis;

namespace Horizon.Returnables;

/// <summary>
/// Represents the outcome of an operation that returns a value of type <typeparamref name="TValue"/>.
/// Wraps either a successful value or an <see cref="Exception"/> — making success and failure
/// explicit in the method signature without relying on thrown exceptions.
/// <para>For operations that do not return a value, use <see cref="Result"/> instead.</para>
/// </summary>
/// <typeparam name="TValue">The type of the value produced on success.</typeparam>
/// <remarks>
/// <para><b>Returning success or failure:</b></para>
/// <code>
/// public Result&lt;User&gt; GetUser(int id)
/// {
///     var user = repository.Find(id);
///
///     if (user is null)
///     {
///         return Result&lt;User&gt;.Fail("User not found");
///     }
///
///     return user; // implicit conversion
/// }
/// </code>
///
/// <para><b>Fluent side-effects:</b></para>
/// <code>
/// GetUser(42)
///     .OnSuccess(user =&gt; logger.LogInformation("Loaded {Id}", user.Id))
///     .OnFailure(error =&gt; logger.LogError(error, "Lookup failed"));
/// </code>
///
/// <para><b>Try/catch wrapping:</b></para>
/// <code>
/// Result&lt;Config&gt; config = Result&lt;Config&gt;.Try(
///     () =&gt; JsonSerializer.Deserialize&lt;Config&gt;(json)!);
/// </code>
///
/// <para>This is a <see langword="struct"/> — stack-allocated when <typeparamref name="TValue"/>
/// is a value type. Accessing <see cref="Value"/> when <see cref="Failed"/> returns
/// <see langword="default"/> — always check <see cref="Succeeded"/> first or use
/// <see cref="Switch"/> for safe unwrapping.</para>
/// </remarks>
public readonly struct Result<TValue>
{
    /// <summary>
    /// A pre-built failure with a generic "Process failed" message.
    /// Prefer <see cref="Fail(string?)"/> or <c>Fail&lt;TError&gt;(TError)</c> for descriptive errors.
    /// </summary>
    public static readonly Result<TValue> Failure;

    /// <summary>
    /// The value produced on success, or <see langword="default"/> on failure.
    /// <para><b>Caution:</b> Do not access without checking <see cref="Succeeded"/> first.
    /// Use <see cref="Switch"/> for safe unwrapping.</para>
    /// </summary>
    public TValue? Value { get; }

    /// <summary>
    /// The exception that caused the failure, or <see langword="null"/> on success.
    /// </summary>
    public Exception? Error { get; }

    /// <summary>
    /// <see langword="true"/> when the operation succeeded and <see cref="Value"/> is available.
    /// After checking this, the compiler knows <see cref="Value"/> is non-null and <see cref="Error"/> is null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool Succeeded => Error is null && Value is not null;

    /// <summary>
    /// <see langword="true"/> when the operation failed and <see cref="Error"/> is available.
    /// After checking this, the compiler knows <see cref="Error"/> is non-null and <see cref="Value"/> is null.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Value))]
    public bool Failed => Error is not null;

    static Result()
    {
        Failure = Fail("Process failed");
    }

    private Result(TValue value)
    {
        Value = value;
        Error = default;
    }

    private Result(Exception error)
    {
        Value = default;
        Error = error;
    }

    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> of a different type from <paramref name="value"/>.
    /// Delegates to <see cref="Result{TValue}.From(TValue?)"/> on the target type.
    /// </summary>
    /// <typeparam name="TNewValue">The target value type.</typeparam>
    /// <param name="value">The value to wrap.</param>
    public static Result<TNewValue> From<TNewValue>(TNewValue? value)
        => Result<TNewValue>.From(value);

    /// <summary>
    /// Creates a <see cref="Result{TValue}"/> from <paramref name="value"/>.
    /// If <paramref name="value"/> is an <see cref="Exception"/>, returns a failure wrapping it.
    /// Otherwise returns a successful result.
    /// </summary>
    /// <param name="value">The value to wrap. Must not be <see langword="null"/> unless it is an <see cref="Exception"/>.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="value"/> is <see langword="null"/> and not an <see cref="Exception"/>.</exception>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result&lt;int&gt;.From(42);        // success
    /// Result&lt;int&gt; result = Result&lt;int&gt;.From(someObject);  // success if non-null, throws if null
    /// </code>
    /// </example>
    public static Result<TValue> From(TValue? value)
    {
        if (value is Exception error)
        {
            return Fail(error);
        }

        ArgumentNullException.ThrowIfNull(value);
        return new(value);
    }

    /// <summary>
    /// Creates a failure from a typed exception, preserving the full exception context
    /// (stack trace, inner exceptions, custom properties).
    /// </summary>
    /// <typeparam name="TError">The exception type.</typeparam>
    /// <param name="error">The exception. Must not be <see langword="null"/>.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="error"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// catch (DatabaseException ex)
    /// {
    ///     return Result&lt;User&gt;.Fail(ex);
    /// }
    /// </code>
    /// </example>
    public static Result<TValue> Fail<TError>(TError? error) where TError : Exception
    {
        ArgumentNullException.ThrowIfNull(error);
        return new(error);
    }

    /// <summary>
    /// Creates a failure from a plain error message.
    /// When <paramref name="message"/> is null/empty/whitespace, uses "Process failed".
    /// </summary>
    /// <param name="message">A human-readable error description.</param>
    /// <example>
    /// <code>
    /// return Result&lt;User&gt;.Fail("Email is required");
    /// </code>
    /// </example>
    public static Result<TValue> Fail(string? message)
    {
        return string.IsNullOrWhiteSpace(message)
            ? new(new Exception("Process failed"))
            : new(new Exception(message));
    }

    /// <summary>
    /// Creates a failure with the default "Process failed" message.
    /// Equivalent to returning <see cref="Failure"/>.
    /// </summary>
    public static Result<TValue> Fail()
    {

        return Failure;
    }

    /// <summary>
    /// Executes one of two actions depending on the outcome.
    /// Guarantees exhaustive handling of both paths.
    /// </summary>
    /// <param name="onSuccess">Invoked with <see cref="Value"/> on success.</param>
    /// <param name="onFailure">Invoked with <see cref="Error"/> on failure.</param>
    /// <exception cref="ArgumentNullException">When either delegate is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// GetUser(42).Switch(
    ///     onSuccess: user =&gt; cache.Set(user.Id, user),
    ///     onFailure: error =&gt; logger.LogError(error, "Lookup failed"));
    /// </code>
    /// </example>
    public void Switch(Action<TValue> onSuccess, Action<Exception> onFailure)
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
    /// Executes <paramref name="action"/> as a side-effect on success, then returns the same result.
    /// Does nothing on failure. Useful for logging or telemetry without breaking a fluent chain.
    /// </summary>
    /// <param name="action">The action to execute with the success value.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="action"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// GetUser(42)
    ///     .OnSuccess(user =&gt; logger.LogInformation("Loaded {Id}", user.Id))
    ///     .OnFailure(error =&gt; logger.LogError(error, "Failed"));
    /// </code>
    /// </example>
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
    /// Executes <paramref name="action"/> as a side-effect on failure, then returns the same result.
    /// Does nothing on success. Useful for logging errors without breaking a fluent chain.
    /// </summary>
    /// <param name="action">The action to execute with the error.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="action"/> is <see langword="null"/>.</exception>
    public Result<TValue> OnFailure(Action<Exception> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (Failed)
        {
            action(Error);
        }

        return this;
    }

    /// <summary>
    /// Wraps a synchronous delegate in a try/catch. Returns the value on success,
    /// or a failure containing the caught exception.
    /// </summary>
    /// <param name="func">The operation to attempt.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="func"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// Result&lt;Config&gt; config = Result&lt;Config&gt;.Try(
    ///     () =&gt; JsonSerializer.Deserialize&lt;Config&gt;(json)!);
    /// </code>
    /// </example>
    public static Result<TValue> Try(Func<TValue> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            return new(func());
        }
        catch (Exception ex)
        {
            return new(ex);
        }
    }

    /// <summary>
    /// Wraps an async delegate in a try/catch. Returns the value on success,
    /// or a failure containing the caught exception.
    /// </summary>
    /// <param name="func">The async operation to attempt.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="func"/> is <see langword="null"/>.</exception>
    /// <example>
    /// <code>
    /// Result&lt;User&gt; result = await Result&lt;User&gt;.TryAsync(
    ///     async () =&gt; await httpClient.GetFromJsonAsync&lt;User&gt;(url));
    /// </code>
    /// </example>
    public static async Task<Result<TValue>> TryAsync(Func<Task<TValue>> func)
    {
        ArgumentNullException.ThrowIfNull(func);

        try
        {
            return new(await func());
        }
        catch (Exception ex)
        {
            return new(ex);
        }
    }

    /// <summary>
    /// Returns a success with <paramref name="value"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or a failure with <paramref name="error"/> when <see langword="false"/>.
    /// Inverse of <c>FailIf&lt;TError&gt;(bool, TValue?, TError?)</c>.
    /// </summary>
    /// <typeparam name="TError">The exception type.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="value">The value to return on success.</param>
    /// <param name="error">The exception to use on failure.</param>
    public static Result<TValue> SuccessIf<TError>(bool condition, TValue? value, TError? error) where TError : Exception
    {
        return FailIf(!condition, value, error);
    }

    /// <summary>
    /// Returns a success with <paramref name="value"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or a failure with <paramref name="errorMessage"/> when <see langword="false"/>.
    /// Inverse of <see cref="FailIf(bool, TValue?, string?)"/>.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="value">The value to return on success.</param>
    /// <param name="errorMessage">The error message on failure.</param>
    /// <example>
    /// <code>
    /// Result&lt;int&gt; result = Result&lt;int&gt;.SuccessIf(
    ///     age &gt;= 18, value: age, errorMessage: "Must be at least 18");
    /// </code>
    /// </example>
    public static Result<TValue> SuccessIf(bool condition, TValue? value, string? errorMessage)
    {
        return FailIf(!condition, value, errorMessage);
    }

    /// <summary>
    /// Returns a failure with <paramref name="error"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or a success with <paramref name="value"/> when <see langword="false"/>.
    /// </summary>
    /// <typeparam name="TError">The exception type.</typeparam>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="value">The value to return on success.</param>
    /// <param name="error">The exception to use on failure.</param>
    public static Result<TValue> FailIf<TError>(bool condition, TValue? value, TError? error) where TError : Exception
        => condition ? Fail(error) : From(value);

    /// <summary>
    /// Returns a failure with <paramref name="errorMessage"/> when <paramref name="condition"/> is <see langword="true"/>,
    /// or a success with <paramref name="value"/> when <see langword="false"/>.
    /// </summary>
    /// <param name="condition">The condition to evaluate.</param>
    /// <param name="value">The value to return on success.</param>
    /// <param name="errorMessage">The error message on failure.</param>
    /// <example>
    /// <code>
    /// Result&lt;string&gt; result = Result&lt;string&gt;.FailIf(
    ///     string.IsNullOrEmpty(email), value: email, errorMessage: "Email is required");
    /// </code>
    /// </example>
    public static Result<TValue> FailIf(bool condition, TValue? value, string? errorMessage)
        => condition ? Fail(errorMessage) : From(value);

    /// <summary>
    /// Converts a <see cref="Result{TValue}"/> to a void <see cref="Result"/>,
    /// preserving the error on failure or returning <see cref="Result.Success"/>.
    /// </summary>
    public static implicit operator Result(Result<TValue> result)
    => result.Failed ? result.Error : Result.Success;

    /// <summary>
    /// Converts an <see cref="Exception"/> into a failed <see cref="Result{TValue}"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// Result&lt;User&gt; result = new KeyNotFoundException("No user found");
    /// </code>
    /// </example>
    public static implicit operator Result<TValue>(Exception error)
        => Fail(error);

    /// <summary>
    /// Unwraps <see cref="Value"/> from the result.
    /// Returns <see langword="default"/> on failure — check <see cref="Succeeded"/> first.
    /// </summary>
    public static implicit operator TValue?(Result<TValue> result)
        => result.Value;

    /// <summary>
    /// Extracts <see cref="Error"/> from the result.
    /// Returns <see langword="null"/> on success.
    /// </summary>
    public static implicit operator Exception?(Result<TValue> result)
        => result.Error;
}