using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WordAnalytics.Cli;
using WordAnalytics.Counting;
using WordAnalytics.Ranking;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWordCounting();
builder.Services.AddWordRanking(builder.Configuration);
builder.Services.AddSingleton<TextAnalysisService>();

using var host = builder.Build();

Console.WriteLine("Your text:");
var input = Console.ReadLine();

var analysis = host.Services.GetRequiredService<TextAnalysisService>();
var words = analysis.Analyze(input);

foreach (var (word, count) in words)
{
    Console.WriteLine($"{word} ({count})");
}