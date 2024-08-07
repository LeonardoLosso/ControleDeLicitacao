using ControleDeLicitacao.App.Upload.Services;
using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;

namespace ControleDeLicitacao.App.Services.Documentos;

public class UploadService
{
    private readonly RequestService _requestService;
    public UploadService(RequestService requestService)
    {
        _requestService = requestService;
    }
    public async Task Upload(IFormFile file)
    {
        //APARENTEMENTE NÃO PARECE UMA BOA IDEIA MANDAR O ARQUIVO DIRETAMENTE, TENTAREMOS COMO UM TEXTO DE FATO 

        var documento = await PdfToString(file);

        if (string.IsNullOrWhiteSpace(documento)) return;

        var template = "ignore as informações das clausulas, " +
            "extraia para mim as seguintes informações presentes em tabela " +
            "(apenas se a tabela possuir realmente valorUnitario e total):" +
            "Caso não encontre as informações a partir das descrições " +
            "pode usar seus proprios requisitos de análise" +
            "Nome string (no campo PRODUTO, possivelmente escrito apos 'in natura, tipo')" +
            "Unidade string (campo UNID) Quantidade number (campo QTDADE)" +
            "ValorUnitario number (campo VALOR UNITÁRIO) ValorTotal number";

        await _requestService.BuildRequest(documento, template);
    }

    private async Task<string> PdfToString(IFormFile file)
    {
        var filePath = Path.GetTempFileName();
        using (var stream = System.IO.File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        string fileContent;
        using (var pdf = PdfDocument.Open(filePath))
        {
            var text = new System.Text.StringBuilder();
            foreach (var page in pdf.GetPages())
            {
                text.Append(page.Text);
            }
            fileContent = text.ToString();
        }

        System.IO.File.Delete(filePath);

        return fileContent;
    }


    //------------------------------------[GEMINI]--------------------------------------



}
