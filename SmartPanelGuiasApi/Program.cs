using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartPanelGuiasApi.Middleware;
using SmartPanelGuiasApi.Services;
using System.Security.Claims;
using System.Text;
using SmartPanelGuiasApi.Conexion;

var builder = WebApplication.CreateBuilder(args);

// -------------------
// ✅ Servicios
// -------------------
builder.Services.AddScoped<DbConexion>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<GuiaService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddAutoMapper(typeof(Program));

// -------------------
// ✅ CORS
// -------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorDev", policy =>
    {
        policy.WithOrigins("https://localhost:7215")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// -------------------
// ✅ JWT: usar clave local o de entorno en producción
// -------------------
string env = builder.Environment.EnvironmentName; // "Development", "Production", etc.
string key;

if (env == "Development")
{
    // 🔹 Clave local fija
    key = "ESTA_ES_MI_CLAVE_SUPER_SECRETA_2026";
}
else
{
    // 🔹 Clave de variable de entorno (Render/Producción)
    key = Environment.GetEnvironmentVariable("JWT_SECRET")
          ?? throw new Exception("JWT_SECRET no definido en variables de entorno");
}

var keyBytes = Encoding.UTF8.GetBytes(key);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

// -------------------
// ✅ Controllers & Swagger con JWT
// -------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SmartPanelGuias API",
        Version = "v1"
    });

    // 🔹 Configuración JWT para Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa tu token JWT generado por /api/Auth/login"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

// -------------------
// 🔹 Construir app
// -------------------
var app = builder.Build();

// -------------------
// ✅ Middleware
// -------------------
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowBlazorDev");
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();

// -------------------
// 🔹 Mapear controllers
// -------------------
app.MapControllers();
app.Run();