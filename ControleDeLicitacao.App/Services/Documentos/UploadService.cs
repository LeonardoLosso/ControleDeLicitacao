using Microsoft.AspNetCore.Http;
using UglyToad.PdfPig;

namespace ControleDeLicitacao.App.Services.Documentos;

public class UploadService
{
    public async Task Upload(IFormFile file)
    {
        //APARENTEMENTE NÃO PARECE UMA BOA IDEIA MANDAR O ARQUIVO DIRETAMENTE, TENTAREMOS COMO UM TEXTO DE FATO 

        var documento = await PdfToString(file);

        if(string.IsNullOrWhiteSpace(documento)) return;

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
}
