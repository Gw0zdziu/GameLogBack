namespace GameLogBack.Exceptions;

public class FailureException : Exception
{
    public string Meesage { get; set; }

    public FailureException(string message) : base(message)
    {
        Meesage = message;
    }
}
