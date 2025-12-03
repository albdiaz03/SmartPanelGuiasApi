using Microsoft.OpenApi;
using SmartPanelGuiasApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorDev", policy =>
    {
        policy.WithOrigins("https://localhost:7053")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<GuiaService>();
builder.Services.AddAutoMapper(typeof(Program));
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

var app = builder.Build();

app.UseCors("AllowBlazorDev");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
