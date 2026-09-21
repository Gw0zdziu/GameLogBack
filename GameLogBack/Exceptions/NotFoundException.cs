using System.Net;

namespace GameLogBack.Exceptions;

public class NotFoundException: AppException
{
    public NotFoundException(string message, string errorCode) : base(message, HttpStatusCode.NotFound, errorCode)
    {
    }
}