namespace Horizon.Returnables;

/// <summary>
/// Represents an error with a code and a message.
/// </summary>
public class Error : Exception
{
    /// <summary>
    /// The error code identifying the type of error.
    /// </summary>
    public string Code { get; protected set; }

    private Error(string code, string message) : base(message)
    {
        Code = code;
    }

    /// <summary>
    /// Creates an error with the specified code and message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    /// <exception cref="ArgumentException">When <paramref name="code"/> or <paramref name="message"/> is null or whitespace.</exception>
    public static Error Create(string? code, string? message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);
        return new Error(code, message);
    }

    /// <summary>
    /// Creates an error from an exception.
    /// </summary>
    /// <param name="exception">The exception to wrap.</param>
    /// <exception cref="ArgumentNullException">When <paramref name="exception"/> is null.</exception>
    public static Error FromException(Exception? exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return exception is Error error
            ? error
            : new Error("UNHANDLED", exception.Message);
    }
}
