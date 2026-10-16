using System.Net;
using GameLogBack.Constants;

namespace GameLogBack.Exceptions;

public class ConflictException : AppException
{
    
    public ConflictException(string message, string errorCode) : base(message, HttpStatusCode.Conflict, errorCode)
    {
    }
}