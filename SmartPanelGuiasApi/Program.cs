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
builder.Services.AddScoped<DbConexion>();       // conexión DB
builder.Services.AddScoped<AuthService>();      // servicio de auth
builder.Services.AddScoped<GuiaService>();      // servicio de guías
builder.Services.AddScoped<LogService>();       // servicio de logs

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
// ✅ JWT
// -------------------
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

// -------------------
// ✅ Controllers & Swagger
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
});

// -------------------
// 🔹 Construir la app
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
// 🔹 Inicializar DB
// -------------------
using (var scope = app.Services.CreateScope())
{
    var dbConexion = scope.ServiceProvider.GetRequiredService<DbConexion>();
    DbInitializer.Initialize(dbConexion); // crea tablas y datos iniciales
}

// -------------------
// 🔹 Mapear controllers y ejecutar
// -------------------
app.MapControllers();
app.Run();