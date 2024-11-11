namespace ControleDeLicitacao.Infrastructure;

public class UserKeeper
{
    private static readonly AsyncLocal<string> _currentUser = new AsyncLocal<string>();

    public string CurrentUser
    {
        get => _currentUser.Value;
        set => _currentUser.Value = value;
    }
}
