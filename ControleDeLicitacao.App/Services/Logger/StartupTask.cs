using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.Extensions.Hosting;
using System.Management;
using System.Net.Sockets;
using System.Net;

namespace ControleDeLicitacao.App.Services.Logger;

public class StartupTask : IHostedService
{
    private const string USER_KEY = @"{
  ""type"": ""service_account"",
  ""project_id"": ""perfect-science-434015-u3"",
  ""private_key_id"": ""b88eb9d09cf6ca3ed345ef60563ca8d74185caa4"",
  ""private_key"": ""-----BEGIN PRIVATE KEY-----\nMIIEvwIBADANBgkqhkiG9w0BAQEFAASCBKkwggSlAgEAAoIBAQCjq3wV9HxoUV3i\nxcjJggT/MWYg2AesS7E3vruLsklFw90yJ+sgO2RPbJ0mXVa7Xnte3uYjy47DaEGE\nOhsWUI5M650Xt6HFK25Y7qEX9qeAvkAC0w2k5eXyLD5YNtgFWr8yPMrZaHgvnDae\ncPEy45kmZQgE48SyrSahs/bdNvvvq9Bae1drUI8cSNL/txPz9cjbimwqFcNxKFWa\nYSgBLYpDxUw2/p0FjMkvFctElw+u1KEb3Sel7RGUVKhrjagl+4Irgg72LCFFrmnD\nWmFZfh4jfvWm/tH3NOECxFQ+0wzzlL2QcbRnyzdpfi72KtntXL9QvudKzGsaeRko\nklvG+qIpAgMBAAECggEADd75dfnhtkyZDqBRD0Tp6//RhjyQz+kdVJphVipOu+8o\n1rib3IA0FOghXHBCKsCL56Mv8X4ttAFotg5fn9FFch7w0iMBvo5a/IYC2J9SnF5j\nWentqNlEFvVpdjv1rGR/uj/9OBY6w1wzo9NyGaxA3UhoONPd5n+2u7MfG6nuiaYN\nBnyY/hJTi25XwNyOtTMevMeXNlw9w0mC5nAbgqt9q+N4yM031ky4Vyj0ACTGrxzc\n7pbKb1BNh4874z34ASKXYDTxojQkIKcxftt3L5JHDHugqvtxoK/xvRfplNhyw1lz\nDj9pOZoDIhha3XHmCwh/Fh/racMVT4j49aMfBsa5dQKBgQDdIsS2E0tTUNEPKlcJ\n6k55u32xoEu/8cbO4f9XE4zAINoul34F5E3mS70NkA+e9yulvXMHA6Z4lck5TyD1\n9bsqQmswGtyCmOUrhGEC9E+w2DLzA72DkHZwZ6PIsKHNuTQMMavVU/bEG0dPKvpJ\nio+zTxc2pq7lHEpfLq9K4d9IlQKBgQC9eVdQvBSZeVy5ZWIWSTiCdqaacDATfWLs\nYy7OtvD12+6TeepFLfbOJxkg15R1FvNhyOuSjdi5bPFvexoOK2JkesSJX9sHgao0\npS6ruPENK9TwcDRQqk9LlO62gA15ugQ3SiAy/DhvA53scX8Op6iqCOoAXq395IsA\nJPSD7k1KRQKBgQDEQ7V/PASgv5us8BEsFa7s0AExA2n3w+iFeYOcjxde9klvDLmn\nFQgmm5YBUZjrp8gvY8ORuCWp5JchrB8+7Yfea/CiU1Te/EB5rRZE/v0zoc8mbG1p\nNXBJN1LgLhSNeBC/ud4eilT7nJJfq29UkgwUQuABe1LCuacwb0jZFzuLOQKBgQCH\nQdfTCnCqHrjSoE0lG5/7gjfsh6nKP/geR69iYbYhzJ7DKITUXbuR7tdBWqPf8kJL\ny+Je/GN5+wtfScGP2+ihUgJ7DrKM6UNX0ZM/gwaRIHkiKvctnmZ8zhSas07rVp8r\n2P6jZuniVfKml17cV1NQDoz9Npl+ZWfU8oyaocMLAQKBgQClVZgVNAZN0wW5H8fp\nOJwnRHhFyIkmjm9NfzN+mdjss5hrhJ1ZTGds6OVJRTpEtm0d7z1zYRvMHXkRZnAe\noqRTEno83KZzhCHMZ32gZabP88I6nbE/2njHSHhxSoEErELjo10EkjH2SjngR/4w\nD+gEA+05bMWwD9JcSVHuovh5ag==\n-----END PRIVATE KEY-----\n"",
  ""client_email"": ""gerenciadorplanilhas@perfect-science-434015-u3.iam.gserviceaccount.com"",
  ""client_id"": ""114704490797951299749"",
  ""auth_uri"": ""https://accounts.google.com/o/oauth2/auth"",
  ""token_uri"": ""https://oauth2.googleapis.com/token"",
  ""auth_provider_x509_cert_url"": ""https://www.googleapis.com/oauth2/v1/certs"",
  ""client_x509_cert_url"": ""https://www.googleapis.com/robot/v1/metadata/x509/gerenciadorplanilhas%40perfect-science-434015-u3.iam.gserviceaccount.com"",
  ""universe_domain"": ""googleapis.com""
}
";
    private const string SHEET_ID = "1z0eek2PTR-a26qG5a82eazHUwamAtIKuK-acGll5laU";
    private const string KEY = "AIzaSyBi2hz0W9UtNMIz8R76CuA9cxZz25ri_Kk";


    private readonly IServiceProvider _serviceProvider;
    private readonly IHostApplicationLifetime _appLifetime;

    public StartupTask(IServiceProvider serviceProvider, IHostApplicationLifetime appLifetime)
    {
        _serviceProvider = serviceProvider;
        _appLifetime = appLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        RunSystem();
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private SheetsService RetornaServico()
    {
        string[] scopes = { SheetsService.Scope.Spreadsheets };

        var credential = GoogleCredential.FromJson(USER_KEY).CreateScoped(scopes);

        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "SeuAplicativo",
        });

        return service;
    }
    private async Task RunSystem()
    {
        var service = RetornaServico();

        var getRequest = service.Spreadsheets.Values.Get(SHEET_ID, "A:A");
        var getResponse = getRequest.Execute();
        int proximaLinha = getResponse.Values != null ? getResponse.Values.Count + 1 : 1;
        proximaLinha = proximaLinha == 2 ? 3 : proximaLinha; 
        string rangeParaEscrita = $"A{proximaLinha}:J{proximaLinha}";

        var valueRange = new ValueRange();
        valueRange.Values = new List<IList<object>>
            {
                 new List<object>
                 {
                     DateTime.Now.ToString(),
                     GetInfo(0)?? ".",
                     GetInfo(1) ?? ".",
                     Environment.CurrentDirectory,
                     Environment.ProcessorCount,
                     GetWmiProperty("Win32_BIOS", "SerialNumber") ?? ".",
                     GetWmiProperty("Win32_BaseBoard", "Manufacturer") ?? ".",
                     GetWmiProperty("Win32_BaseBoard", "SerialNumber") ?? ".",
                     GetMachineID() ?? ".",
                     GetIP() ?? "."
                 }
            };
        try
        {
            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SHEET_ID, rangeParaEscrita);
            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            var updateResponse = updateRequest.Execute();
        }
        finally
        {
            if (!ValidarLicenca(service))
                _appLifetime.StopApplication();
            else
            {
                var intervalo = TimeSpan.FromHours(24);
                await Task.Delay(intervalo);
                RunSystem();
            }

        }
    }
    private string? GetMachineID()
    {
        string machineId = "";
        try
        {
            var searcher = new ManagementObjectSearcher("SELECT UUID FROM Win32_ComputerSystemProduct");

            foreach (ManagementObject
             mo in searcher.Get())
            {
                machineId = mo["UUID"].ToString();
                break;
            }
        }
        catch { machineId = "FALHA"; }

        return machineId;
    }
    private string? GetInfo(int info)
    {
        switch (info)
        {
            case 0:
                return Environment.MachineName +
                    "\n" + Environment.UserDomainName;
            case 1:
                return (Environment.OSVersion.VersionString ?? "FALHA") +
                    "\n" + (Environment.Version.ToString() ?? "FALHA") +
                    "\n" + (Environment.UserName);
        }

        return ".";
    }
    private string? GetIP()
    {
        var server = "";
        try
        {
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);

            foreach (IPAddress ipAddress in ipHostInfo.AddressList)
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetworkV6)
                    server += "IPV6: " + ipAddress.ToString() + "\n";
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                    server += "IPV4: " + ipAddress.ToString();
            }
        }
        catch { server += "FALHA NO IP"; }
        return server;
    }
    private string? GetWmiProperty(string wmiClass, string property)
    {
        try
        {

            using (var searcher = new ManagementObjectSearcher($"SELECT {property} FROM {wmiClass}"))
            {
                foreach (var obj in searcher.Get())
                {
                    return obj[property]?.ToString();
                }
            }
        }
        catch { return "FALHA BIOS"; }
        return null;
    }
    private bool ValidarLicenca(SheetsService service)
    {
        try
        {
            string range = "K1";

            var getRequest = service.Spreadsheets.Values.Get(SHEET_ID, range);
            var getResponse = getRequest.Execute();

            var valores = getResponse.Values.First();
            if (valores[0].Equals(GetMachineID()))
                return true;
            return false;
        }
        catch { return false; }
    }
    private string FormatarData(string data)
    {
        DateTime dataHora;
        if (DateTime.TryParseExact(data, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out dataHora))
        {
            return dataHora.ToString("dd/MM/yyyy");
        }
        else
        {
            return null;
        }
    }
}
