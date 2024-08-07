namespace ControleDeLicitacao.App.Upload.Dictionary;

public class Roles
{
    public const string User = "user";
    public const string Model = "model";

    public static readonly Dictionary<string, string> AllRoles = new Dictionary<string, string>
    {
        { "User", User },
        { "Model", Model }
    };
}
public class ResponseType
{
    public const string Json = "application/json";

    public static readonly Dictionary<string, string> AllRoles = new Dictionary<string, string>
    {
        { "Json", Json }
    };
}
