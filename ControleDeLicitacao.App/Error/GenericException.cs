namespace ControleDeLicitacao.App.Error;

public class GenericException: Exception
{
    public int StatusCode { get; set; }

    public GenericException(string message, int statusCode = 400) : base(message)
    {
        StatusCode = statusCode;
    }
}
