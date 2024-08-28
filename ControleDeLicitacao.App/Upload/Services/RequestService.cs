using ControleDeLicitacao.App.Error;
using ControleDeLicitacao.App.Upload.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ControleDeLicitacao.App.Upload.Services;

public class RequestService
{
    private readonly HttpClient _httpClient;
    private const string API_KEY = "AIzaSyBYejOrL2iQAMM2GkIRRYfTAvjGIDO0YRc";
    public RequestService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _httpClient.Timeout = TimeSpan.FromMinutes(5);
    }

    public async Task<RootResponse> BuildRequest(string document, string template)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={API_KEY}";
        var request = new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var body = RetornaRequestBody(document, template);

        var json = RequestToJSON(body);

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;
        var response = new HttpResponseMessage();
        try
        {
            response = await _httpClient.SendAsync(request);
        }
        catch (Exception ex)
        {
            throw new GenericException(ex.Message, 501);
        }

        if (!response.IsSuccessStatusCode) 
            throw new GenericException("ERRO COM A REQUISIÇÃO", 501);

        var responseContent = await response.Content.ReadAsStringAsync();

        if(string.IsNullOrWhiteSpace(responseContent)) throw new GenericException("NÃO FOI POSSIVEL LER A RESPOSTA DA API GOOGLE", 501);

        var retorno = RetornaResponseBody(responseContent);

        return retorno;
    }
    private RootResponse RetornaResponseBody(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        var body = JsonSerializer.Deserialize<RootResponse>(json, options);

        if (body is null) throw new GenericException("RETORNO DA I.A. FORA DO PADRÃO", 501);
        if (body.Candidates[0].FinishReason.Equals("MAX_TOKENS")) throw new GenericException("Documento grande demais para a I.A. processar", 501);
        return body;
    }
    private RootRequest RetornaRequestBody(string document, string template)
    {
        var root = new RootRequest();
        root.Contents[0].Parts.Add(new Part(document));

        root.SystemInstruction.Parts.Add(new Part(template));

        return root;
    }

    private string RequestToJSON(RootRequest body)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(body, options);
        return json;
    }
}
