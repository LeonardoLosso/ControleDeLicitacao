using ControleDeLicitacao.App.Upload.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ControleDeLicitacao.App.Upload.Services;

public class RequestService
{
    private readonly HttpClient _httpClient;
    private const string API_KEY = "AIzaSyBi2hz0W9UtNMIz8R76CuA9cxZz25ri_Kk";
    public RequestService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task BuildRequest(string document, string template)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={API_KEY}";
        var request = new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        var body = RetornaBody(document, template);

        var json = RetornaJSON(body);

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        request.Content = content;

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();

        }
    }
    private RootRequest RetornaBody(string document, string template)
    {
        var root = new RootRequest();
        root.Contents[0].Parts.Add(new Part(document));

        root.SystemInstruction.Parts.Add(new Part(template));

        return root;
    }

    private string RetornaJSON(RootRequest body)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(body, options);
        return json;
    }
}
