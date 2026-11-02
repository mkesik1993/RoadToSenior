using WordAnalytics.Api.Features.AnalyzeText;
using WordAnalytics.Api.Features.CountWords;
using WordAnalytics.Api.Infrastructure;
using WordAnalytics.Api.Infrastructure.HealthChecks;
using WordAnalytics.Api.Infrastructure.RateLimiting;
using WordAnalytics.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication(builder.Configuration);

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
app.MapCountWordsEndpoint();

app.Run();

public partial class Program { }