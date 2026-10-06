using WordAnalytics.Api.Controllers;
using WordAnalytics.Api.Infrastructure;
using WordAnalytics.Api.Infrastructure.HealthChecks;
using WordAnalytics.Api.Infrastructure.RateLimiting;
using WordAnalytics.Counting;
using WordAnalytics.Ranking;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWordCounting();
builder.Services.AddWordRanking(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApiHealthChecks();
builder.Services.AddApiRateLimiting(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRateLimiter();

app.MapApiHealthChecks();
app.MapAnalysisEndpoints();

app.Run();

public partial class Program { }