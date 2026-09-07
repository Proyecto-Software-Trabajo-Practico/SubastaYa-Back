using Application.DTOs;
using Application.Interfaces;
using Application.Mediators;
using Application.UseCases.Categorias.Handlers;
using Application.UseCases.Categorias.Queries;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración de CORS (Intercambio de recursos de origen cruzado) para el Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173" // React / Vite 
              )
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Registro de servicios de la capa Infrastructure
builder.Services.AddDataServices(builder.Configuration);
// Registramos el Handler para que pueda ser inyectado en el Mediator
builder.Services.AddScoped<IRequestHandler<ObtenerCategoriasQuery, List<CategoriaDto>>, ObtenerCategoriasQueryHandler>();
// Registramos el Mediador: cuando alguien pida IMediator, .NET le entrega una instancia de Mediator
builder.Services.AddScoped<IMediator, Mediator>();
// Para activar los Controller
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar la política de CORS (debe ir antes de los endpoints)
app.UseCors("AllowFrontend");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapControllers();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
