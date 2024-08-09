using System.Globalization;
using System.Text;

namespace ControleDeLicitacao.Common.Enum;

public static class EstadosBrasil
{
    private static readonly Dictionary<string, string> estados = new Dictionary<string, string>
    {
        { "ACRE", "AC" },
        { "ALAGOAS", "AL" },
        { "AMAPA", "AP" },
        { "AMAZONAS", "AM" },
        { "BAHIA", "BA" },
        { "CEARA", "CE" },
        { "DISTRITOFEDERAL", "DF" },
        { "ESPIRITOSANTO", "ES" },
        { "GOIAS", "GO" },
        { "MARANHAO", "MA" },
        { "MATOGROSSO", "MT" },
        { "MATOGROSSODOSUL", "MS" },
        { "MINASGERAIS", "MG" },
        { "PARA", "PA" },
        { "PARAIBA", "PB" },
        { "PARANA", "PR" },
        { "PERNAMBUCO", "PE" },
        { "PIAUI", "PI" },
        { "RIODEJANEIRO", "RJ" },
        { "RIOGRANDEDONORTE", "RN" },
        { "RIOGRANDEDOSUL", "RS" },
        { "RONDONIA", "RO" },
        { "RORAIMA", "RR" },
        { "SANTACATARINA", "SC" },
        { "SAOPAULO", "SP" },
        { "SERGIPE", "SE" },
        { "TOCANTINS", "TO" }
    };

    public static string ObterSigla(string nomeEstado)
    {
        if (string.IsNullOrWhiteSpace(nomeEstado))
            return "";

        var normalizedString = RemoverAcentos(nomeEstado.ToUpperInvariant());

        normalizedString = string.Concat(normalizedString.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

        if (estados.TryGetValue(normalizedString, out var sigla))
        {
            return sigla;
        }

        return "";
    }

    private static string RemoverAcentos(string texto)
    {
        return string.Concat(texto.Normalize(NormalizationForm.FormD)
                                  .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
                     .Normalize(NormalizationForm.FormC);
    }
}
