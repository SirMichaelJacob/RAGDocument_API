
using Microsoft.EntityFrameworkCore;
using RAGDocument.Application.DTOs;
using RAGDocument.Application.Handlers;
using RAGDocument.Application.Interfaces;
using RAGDocument.Infrastructure.Services;
using RAGDocument.Domain.Interfaces;
using RAGDocument.Infrastructure.Persistence;
using RAGDocument.Infrastructure.Repositories;
using System;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.FluentValidation;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("RAGDbConn")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmbeddingService, EmbeddingService>();
builder.Services.AddScoped<IRagService,RAGSystem>();
builder.Services.Configure<LlmSettings>(
    builder.Configuration.GetSection("LLMS"));

// Register HttpClient for EmbeddingService 
builder.Services.AddHttpClient<EmbeddingService>(client =>
{
    var endpointUrl = builder.Configuration["LLMS:EndPointUrl"] ?? "http://localhost:1234";
    client.BaseAddress = new Uri(endpointUrl);

    var timeoutSeconds = builder.Configuration.GetValue<int>("LLMS:TimeoutSeconds", 600);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});

builder.Services.AddHttpClient<LMStudioService>(client =>
{
    var endpointUrl = builder.Configuration["LLMS:EndPointUrl"] ?? "http://localhost:1234";
    client.BaseAddress = new Uri(endpointUrl);

    var timeoutSeconds = builder.Configuration.GetValue<int>("LLMS:TimeoutSeconds", 600);
    client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
});


// Wolverine configuration
builder.Host.UseWolverine(opts =>
{
    opts.UseEntityFrameworkCoreTransactions();
    opts.Discovery.IncludeAssembly(typeof(AppDbContext).Assembly);
    opts.Discovery.IncludeAssembly(typeof(DocumentHandler).Assembly);
    opts.UseFluentValidation();
    //opts.OptimizeArtifactWorkflow();
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
//builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
