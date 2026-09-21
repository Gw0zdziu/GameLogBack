using System.Net;
using GameLogBack.Constants;

namespace GameLogBack.Exceptions;

public class BadRequestException: AppException
{
    public BadRequestException(string message, string errorCode) : base(message, HttpStatusCode.BadRequest, errorCode)
    {
    }
}