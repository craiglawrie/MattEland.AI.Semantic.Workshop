﻿using Azure.AI.OpenAI;
using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.ComponentModel;
using System.Reflection;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

public static class DisplayHelpers
{

    public static void DisplayBorderedMessage(string header, string message)
    {
        DisplayBorderedMessage(header, message, Color.SteelBlue);
    }

    public static void DisplayBorderedMessage(string header, string message, Color borderColor)
    {
        AnsiConsole.Write(new Panel(message)
                       .Header(header)
                       .BorderStyle(new Style(borderColor))
                       .Expand());
        AnsiConsole.WriteLine();
    }

    public static void DisplayBorderedMessage(string header, IRenderable content)
    {
        DisplayBorderedMessage(header, content, Color.SteelBlue);
    }

    public static void DisplayBorderedMessage(string header, IRenderable content, Color borderColor)
    {
        AnsiConsole.Write(new Panel(content)
                       .Header(header)
                       .BorderStyle(new Style(borderColor))
                       .Expand());
        AnsiConsole.WriteLine();
    }

    public static string ToFriendlyName(this Enum value)
    {
        FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());
        DescriptionAttribute[]? attributes = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
        return attributes?[0].Description ?? value.ToString();
    }

    public static void DisplayImage(this string imagePath)
    {
        CanvasImage image = new(imagePath);
        image.MaxWidth = 30;
        AnsiConsole.Write(image);
    }

    public static async Task DisplayImageAsync(this Uri imageUri)
    {
        using HttpClient webClient = new();
        using Stream stream = await webClient.GetStreamAsync(imageUri);

        DisplayImage(stream);
    }

    public static void DisplayImage(this Stream imageStream)
    {
        CanvasImage image = new(imageStream);
        image.MaxWidth = 30;
        AnsiConsole.Write(image);
    }

    public static void DisplayContentFilterResults(ContentFilterResultDetailsForPrompt filter)
    {
        Table contentTable = new();
        contentTable.Title = new TableTitle("[Yellow]Content Filter Results[/]");

        contentTable.AddColumns("Type", "Severity", "Filtered?");
        AddContentFilterRow(contentTable, filter.Sexual, "Sexual");
        AddContentFilterRow(contentTable, filter.Violence, "Violence");
        AddContentFilterRow(contentTable, filter.SelfHarm, "Self-Harm");

        AnsiConsole.Write(contentTable);
        AnsiConsole.WriteLine();
    }

    private static void AddContentFilterRow(Table table, ContentFilterResult result, string name)
    {
        string severity = result.Severity.ToString();

        severity = severity switch
        {
            "safe" => "[Green]Safe[/]",
            "low" => "[Yellow]Low[/]",
            "medium" => "[Orange3]Medium[/]",
            "high" => "[Red]High[/]",
            _ => severity
        };

        table.AddRow($"[SteelBlue]{name}[/]", severity, result.Filtered ? "[Red]Yes[/]" : "[Green]No[/]");
    }

    public static void DisplayTokenUsage(CompletionsUsage usage)
    {
        DisplayBorderedMessage($"[White]Token Usage ({usage.TotalTokens} Tokens)[/]", new BreakdownChart()
            .FullSize()
            .AddItem("Prompt", usage.PromptTokens, Color.Yellow)
            .AddItem("Completion", usage.CompletionTokens, Color.SteelBlue));
    }
}
