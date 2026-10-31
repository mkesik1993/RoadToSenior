using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WordAnalytics.Application;
using WordAnalytics.Application.Features.AnalyzeText;
using WordAnalytics.Counting;
using WordAnalytics.Ranking;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddWordCounting();
builder.Services.AddWordRanking(builder.Configuration);
builder.Services.AddApplication();

using var host = builder.Build();

Console.WriteLine("Your text:");
var input = Console.ReadLine();

var analysis = host.Services.GetRequiredService<AnalyzeTextHandler>();
var result = analysis.Handle(new AnalyzeTextInput(input));

foreach (var (word, count) in result.Top)
{
    Console.WriteLine($"{word} ({count})");
}