using System.Text.RegularExpressions;

namespace ControleDeLicitacao.Common;

public static class StringExtensions
{
    public static string RemoveNonNumeric(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        return Regex.Replace(input, @"\D", string.Empty);
    }
}
