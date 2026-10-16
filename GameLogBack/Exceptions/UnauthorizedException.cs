using System.Net;
using GameLogBack.Constants;

namespace GameLogBack.Exceptions;

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message, string errorCode) : base(message, HttpStatusCode.Unauthorized, errorCode)
    {
    }
}