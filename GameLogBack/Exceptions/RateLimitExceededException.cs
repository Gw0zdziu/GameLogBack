using System.Net;

namespace GameLogBack.Exceptions;

public class RateLimitExceededException : AppException
{
    public RateLimitExceededException(string message) : base(message, HttpStatusCode.TooManyRequests)
    {
    }
}
