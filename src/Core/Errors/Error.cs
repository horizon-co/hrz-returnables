using Horizon.Returnables.Core.Errors.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Horizon.Returnables.Core.Errors;

[DataContract]
public record Error
{
    [DataMember, JsonPropertyName("code")]
    public string Code { get; }

    [DataMember, JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ErrorType Type { get; }

    [DataMember, JsonPropertyName("message")]
    public string Message { get; }

    /// <summary>
    /// Initialize the error with code and message description
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public Error(string code, string message, ErrorType type = ErrorType.BusinessError)
    {
        Type = type;
        Code = string.IsNullOrWhiteSpace(code) ? throw new ArgumentNullException(nameof(code)) : code;
        Message = string.IsNullOrWhiteSpace(message) ? throw new ArgumentNullException(nameof(message)) : message;
    }

    public static Error Create(string code, string message, ErrorType type = ErrorType.BusinessError)
        => new(code, message, type);

    public override string ToString()
        => $"Code: {Code}\nMessage: {Message}\nType: {Type}";

    public static implicit operator string(Error error)
        => error.ToString();
}