namespace Horizon.Returnables.Core.Errors;

public sealed record CommonError : Error
{
    /// <summary>
    /// Initiate an generic error with default code and message
    /// </summary>
    public CommonError() : base("001", "An error occurred") { }
}