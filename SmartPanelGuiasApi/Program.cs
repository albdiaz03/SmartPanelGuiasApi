using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SmartPanelGuiasApi.Middleware;
using SmartPanelGuiasApi.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ?? CORS para Blazor
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorDev", policy =>
    {
        policy.WithOrigins("https://localhost:7215")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ?? Servicios
builder.Services.AddScoped<GuiaService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddAutoMapper(typeof(Program));

// ?? JWT
var key = "ESTA_ES_MI_CLAVE_SUPER_SECRETA_2026";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddControllers();

// Servicio de log
builder.Services.AddScoped<LogService>();


// ?? Swagger
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

// ?? Middleware global de errores
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowBlazorDev");

app.UseAuthentication();   // ?? SIEMPRE antes
app.UseAuthorization();    // ?? SOLO UNA VEZ

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
