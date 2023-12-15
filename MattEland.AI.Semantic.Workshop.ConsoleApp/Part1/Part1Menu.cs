﻿using MattEland.AI.Semantic.Workshop.ConsoleApp.Part1;
using Spectre.Console;

namespace MattEland.AI.Semantic.Workshop.ConsoleApp;

public class Part1Menu
{
    private readonly TextAnalysisDemo _textAnalysis;
    private readonly ImageAnalysisDemo _imageAnalysis;

    public Part1Menu(Part1Settings settings)
    {
        _textAnalysis = new TextAnalysisDemo(settings.AiEndpoint, settings.AiKey);
        _imageAnalysis = new ImageAnalysisDemo(settings.AiEndpoint, settings.AiKey);
    }

    public async Task RunAsync()
    {
        Dictionary<string, Func<string>> textSources = new()
        {
            { "Custom text", () => AnsiConsole.Prompt<string>(new TextPrompt<string>("[Yellow]Enter your own text to analyze:[/]")) },
            { "This Workshop's Abstract", () => Properties.Resources.WorkshopAbstract},
            { "Semantic Kernel Announcement", () => Properties.Resources.SemanticKernelAnnouncement },
            { "Back", () => string.Empty }
        };

        Dictionary<string, Func<string>> imageSources = new()
        {
            { "AI Generated Portrait (local file)", () => "Resources/AIPortrait.png"},
            { "Article Billboard (web file)", () => "https://accessibleai.dev/img/SK/A_SemanticKernelIntro.png" },
            { "Custom Image", () => AnsiConsole.Prompt<string>(new TextPrompt<string>("[Yellow]Enter the image URL or relative path:[/]")) },
            { "Back", () => string.Empty }
        };

        bool hasQuit = false;
        while (!hasQuit)
        {
            Part1MenuOptions choice = AnsiConsole.Prompt(new SelectionPrompt<Part1MenuOptions>()
                .Title("What task in part 1?")
                .HighlightStyle(Style.Parse("Orange3"))
                .AddChoices(Enum.GetValues(typeof(Part1MenuOptions)).Cast<Part1MenuOptions>())
                .UseConverter(c => c.ToFriendlyName()));

            switch (choice)
            {
                case Part1MenuOptions.AnalyzeText:
                    string textToAnalyze = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                               .Title("What text do you want to analyze?")
                                               .HighlightStyle(Style.Parse("Orange3"))
                                               .AddChoices(textSources.Keys)
                                               .UseConverter(c => c));

                    if (textToAnalyze == "Back") break;

                    AnsiConsole.MarkupLine($"[Yellow]Analyzing {textToAnalyze}[/]");
                    string documentText = textSources[textToAnalyze]();

                    await _textAnalysis.AnalyzeAsync(documentText);
                    break;

                case Part1MenuOptions.AnalyzeImage:
                    string pathToAnalyze = AnsiConsole.Prompt(new SelectionPrompt<string>()
                                               .Title("What image do you want to analyze?")
                                               .HighlightStyle(Style.Parse("Orange3"))
                                               .AddChoices(imageSources.Keys)
                                               .UseConverter(c => c));

                    if (pathToAnalyze == "Back") break;

                    string imageSource = imageSources[pathToAnalyze]();

                    await _imageAnalysis.AnalyzeAsync(imageSource);
                    break;

                case Part1MenuOptions.TextToSpeech:
                    AnsiConsole.WriteLine("Text to speech is not yet implemented. Please check back later.");
                    break;

                case Part1MenuOptions.SpeechToText:
                    AnsiConsole.WriteLine("Speech to text is not yet implemented. Please check back later.");
                    break;

                case Part1MenuOptions.Back:
                    hasQuit = true;
                    break;

                default:
                    AnsiConsole.WriteLine($"Matt apparently forgot to handle menu choice {choice}. What a dolt!");
                    break;
            }

            AnsiConsole.WriteLine();
        }
    }
}