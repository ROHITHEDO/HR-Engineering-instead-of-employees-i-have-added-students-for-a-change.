using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentManagement.Application.Interfaces;
using StudentManagement.Application.Services;
using StudentManagement.Infrastructure.AI;
using StudentManagement.Infrastructure.BlobStorage;
using StudentManagement.Infrastructure.Data;
using StudentManagement.Infrastructure.Identity;
using StudentManagement.Infrastructure.Messaging;
using StudentManagement.Infrastructure.Repositories;
using StudentManagement.Infrastructure.Search;
using System.Text;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
builder.Services.AddScoped<IStudentDocumentRepository,StudentDocumentRepository>();
builder.Services.AddScoped<IStudentDocumentService,StudentDocumentService>();
builder.Services.AddScoped<IStudentEventPublisher,ServiceBusStudentEventPublisher>();
builder.Services.AddScoped<IVectorSearchService, QdrantVectorSearchService>();

builder.Services.AddScoped<IDocumentTextExtractor,DocumentTextExtractor>();
builder.Services.AddScoped<IStudentDocumentSearchService,StudentDocumentSearchService>();
builder.Services.AddScoped<ITextChunker,TextChunker>();
builder.Services.AddHttpClient<ILanguageModelService,OllamaLanguageModelService>(
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["Ollama:BaseUrl"]!);
    });

builder.Services.AddScoped<IStudentRagService,StudentRagService>();

//builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>(
//    client =>
//    {
//        client.BaseAddress =
//            new Uri("http://localhost:11434");
//    });

builder.Services.AddHttpClient<IEmbeddingService, OllamaEmbeddingService>(
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["Ollama:BaseUrl"]!);
    }
);

var ollamaBaseUrl =
    builder.Configuration["Ollama:BaseUrl"]
    ?? throw new InvalidOperationException(
        "Ollama BaseUrl is not configured.");

var chatModel =
    builder.Configuration["Ollama:ChatModel"]
    ?? throw new InvalidOperationException(
        "Ollama ChatModel is not configured.");

builder.Services
    .AddKernel()
    .AddOpenAIChatCompletion(
        modelId: chatModel,
        apiKey: "ollama",
        endpoint: new Uri($"{ollamaBaseUrl.TrimEnd('/')}/v1")); 

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste just the JWT (no \"Bearer \" prefix) — Swagger adds that for you."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var jwtSection =
    builder.Configuration.GetSection("Jwt");

var jwtKey =
    jwtSection["Key"]!;

builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    jwtSection["Issuer"],

                ValidAudience =
                    jwtSection["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();