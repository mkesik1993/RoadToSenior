using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WordAnalytics.Application;
using WordAnalytics.Application.Features.AnalyzeText;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication(builder.Configuration);

using var host = builder.Build();

Console.WriteLine("Your text:");
var input = Console.ReadLine();

var analysis = host.Services.GetRequiredService<AnalyzeTextHandler>();
var result = analysis.Handle(new AnalyzeTextInput(input));

foreach (var (word, count) in result.Top)
{
    Console.WriteLine($"{word} ({count})");
}