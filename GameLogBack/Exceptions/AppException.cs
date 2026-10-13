using System.Net;
using GameLogBack.Constants;

namespace GameLogBack.Exceptions;

public abstract class AppException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public string ErrorCode { get; set; }


    protected AppException(string message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError, string errorCode = Constants.ErrorCodes.Internal.InternalServerError) : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
    
}