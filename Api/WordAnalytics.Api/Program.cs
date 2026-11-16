using WordAnalytics.Api.Features.AnalyzeText;
using WordAnalytics.Api.Features.CreateAnalysis;
using WordAnalytics.Api.Features.CountWords;
using WordAnalytics.Api.Features.GetAnalysis;
using WordAnalytics.Api.Features.GetTopWords;
using WordAnalytics.Application;
using WordAnalytics.Infrastructure;
using WordAnalytics.Api.Hosting;
using WordAnalytics.Api.Hosting.HealthChecks;
using WordAnalytics.Api.Hosting.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

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
app.MapAnalyzeTextEndpoint();
app.MapCreateAnalysisEndpoint();
app.MapCountWordsEndpoint();
app.MapGetAnalysisEndpoint();
app.MapGetTopWordsEndpoint();

app.Run();

public partial class Program { }