using Microsoft.OpenApi;
using SmartPanelGuiasApi.Middleware;
using SmartPanelGuiasApi.Services;

var builder = WebApplication.CreateBuilder(args);

// CORS para Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorDev", policy =>
    {
        policy.WithOrigins("https://localhost:7215")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Tus servicios actuales
builder.Services.AddScoped<GuiaService>();

// Agregamos AuthService para login simple
builder.Services.AddScoped<AuthService>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartPanelGuias API",
        Version = "v1"
    });
});

var app = builder.Build();

// Middleware global de errores
app.UseMiddleware<ErrorHandlingMiddleware>();

// CORS
app.UseCors("AllowBlazorDev");

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
