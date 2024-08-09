namespace LibGetoptLike;

public class GetoptException : Exception
{
    public GetoptException(string message) : base(message) { }

    public GetoptException(string message, Exception innerException)
        : base(message, innerException) { }
}
