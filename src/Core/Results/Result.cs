using Horizon.Returnables.Core.Errors;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Horizon.Returnables.Core.Results;

[DataContract]
public record Result<TResult>
{
    [DataMember, JsonPropertyName("success")]
    public bool Success { get; }

    [DataMember, JsonPropertyName("data")]
    public TResult? Data { get; }

    [DataMember, JsonPropertyName("error")]
    public Error? Error { get; }

    public Result()
    {
        Data = default;
        Error = default;
        Success = false;
    }

    /// <summary>
    /// Initialize a result with content
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public Result(TResult? data)
    {
        Success = true;
        Data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <summary>
    /// Initializes a result containing an error
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public Result(Error? error)
    {
        Success = false;
        Error = error ?? throw new ArgumentNullException(nameof(error));
    }

    public static Result<TResult> Create(TResult? result)
        => new(result);

    /// <summary>
    /// Initializes a result containing an error
    /// </summary>
    public static Result<TResult> Create(Error? error)
        => new(error);

    public static implicit operator Result<TResult>(TResult result)
        => new(result);

    public static implicit operator Result<TResult>(Error error)
        => new(error);

    public static implicit operator TResult(Result<TResult> result)
        => result.Data!;

    public static implicit operator Error(Result<TResult> result)
        => result.Error!;
}