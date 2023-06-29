using Horizon.Returnables.Core.Errors;
using System.Net;

namespace Horizon.Returnables.Core.Results;

public sealed record HttpResult<TResult> : Result<TResult>
{
    public HttpStatusCode StatusCode { get; }

    public HttpResult(HttpStatusCode statusCode)
    {
        StatusCode = statusCode;
    }

    public HttpResult(HttpStatusCode statusCode, TResult? data) : base(data)
    {
        StatusCode = statusCode;
    }

    public HttpResult(HttpStatusCode statusCode, Error? error) : base(error)
    {
        StatusCode = statusCode;
    }

    public static HttpResult<TResult> Create(HttpStatusCode statusCode, TResult data)
        => new(statusCode, data);

    public static HttpResult<TResult> Create(HttpStatusCode statusCode, Error error)
        => new(statusCode, error);

    /// <summary>
    /// Initialize a result with HTTP status 200
    /// </summary>
    public static HttpResult<TResult> OK(TResult data)
        => new(HttpStatusCode.OK, data);

    /// <summary>
    /// Initialize a result with HTTP status 201
    /// </summary>
    public static HttpResult<TResult> Created(TResult data)
        => new(HttpStatusCode.Created, data);

    /// <summary>
    /// Initialize a result with HTTP status 204
    /// </summary>
    public static HttpResult<TResult> NoContent()
        => new(HttpStatusCode.NoContent);

    /// <summary>
    /// Initialize a result with HTTP status 400
    /// </summary>
    public static HttpResult<TResult> BadRequest(Error error)
        => new(HttpStatusCode.BadRequest, error);

    /// <summary>
    /// Initialize a result with HTTP status 401
    /// </summary>
    public static HttpResult<TResult> Unauthorized(Error error)
        => new(HttpStatusCode.Unauthorized, error);

    /// <summary>
    /// Initialize a result with HTTP status 403
    /// </summary>
    public static HttpResult<TResult> Forbidden(Error error)
        => new(HttpStatusCode.Forbidden, error);

    /// <summary>
    /// Initialize a result with HTTP status 404
    /// </summary>
    public static HttpResult<TResult> NotFound()
        => new(HttpStatusCode.NotFound);

    /// <summary>
    /// Initialize a result with HTTP status 409
    /// </summary>
    public static HttpResult<TResult> Conflict(Error error)
        => new(HttpStatusCode.Conflict, error);

    /// <summary>
    /// Initialize a result with HTTP status 500
    /// </summary>
    public static HttpResult<TResult> ServerError(Error error)
        => new(HttpStatusCode.InternalServerError, error);

    public static implicit operator HttpStatusCode(HttpResult<TResult> data)
        => data.StatusCode;
}