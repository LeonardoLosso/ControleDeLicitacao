﻿namespace ControleDeLicitacao.App.Upload.Models;
using ControleDeLicitacao.App.Upload.Dictionary;

public class RootRequest
{
    public List<Content> Contents { get; set; }
    public Content SystemInstruction { get; set; }
    public GenerationConfig GenerationConfig { get; set; }

    public RootRequest()
    {
        Contents = new List<Content>();
        Contents.Add(new Content(Roles.User));

        SystemInstruction = new Content(Roles.User);
        GenerationConfig = new GenerationConfig();
    }
}

public class RootResponse
{
    public List<Candidates> Candidates { get; set; }
    public UsageMetadata UsageMetadata { get; set; }
}

public class Content
{
    public string Role { get; set; }
    public List<Part> Parts { get; set; }
    public Content(string role)
    {
        Role = role;
        Parts = new List<Part>();
    }
}

public class Part
{
    public string Text { get; set; } = string.Empty;

    public Part(string text)
    {
        Text = text;
    }
}

public class GenerationConfig
{
    public int Temperature { get; set; } = 1;
    public int TopK { get; set; } = 64;
    public double TopP { get; set; } = 0.95;
    public int MaxOutputTokens { get; set; } = 8192;
    public string ResponseMimeType { get; set; } = ResponseType.Json;
}

public class Candidates
{
    public Content Content { get; set; }
    public string FinishReason { get; set; }
    public int Index { get; set; }
    public List<SafetyRatings> SafetyRatings { get; set; }
}

public class SafetyRatings
{
    public string Category { get; set; }
    public string Probability { get; set; }
}

public class UsageMetadata
{
    public int PromptTokenCount { get; set; }
    public int CandidatesTokenCount { get; set; }
    public int TotalTokenCount { get; set; }
}