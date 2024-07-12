namespace ControleDeLicitacao.App.Error;

public class GenericException: Exception
{
    public int StatusCode { get; set; }
    public Exception Exception2 { get; set; }

    public GenericException(string message, int statusCode = 400, Exception innerException = null) : base(message)
    {
        StatusCode = statusCode;
        Exception2 = innerException;
    }
}
