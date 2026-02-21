using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartPanelGuiasApi.Middleware;
using SmartPanelGuiasApi.Services;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SmartPanelGuiasApi.Conexion;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DbConexion>();
    DbInitializer.Initialize(db);
}

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
builder.Services.AddScoped<DbConexion>(); // 🔹 nueva clase de conexión
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<GuiaService>();

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
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

// ?? Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddScoped<LogService>();
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

// ?? Middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowBlazorDev");
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

// ?? POBLAR DB al iniciar (opcional)
using (var scope = app.Services.CreateScope())
{
    var dbConexion = scope.ServiceProvider.GetRequiredService<DbConexion>();
    DbInitializer.Initialize(dbConexion); // Esto creará tablas y datos iniciales
}

app.MapControllers();
app.Run();