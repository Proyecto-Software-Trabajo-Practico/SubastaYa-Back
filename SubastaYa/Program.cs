using Application.DTOs;
using Application.Interfaces;
using Application.Mediators;
using Application.UseCases.Categorias.Handlers;
using Application.UseCases.Categorias.Queries;
using Application.UseCases.Usuario.Commands;
using Domain.Entities;
using Application.UseCases.Billeteras.Commands;
using Application.UseCases.Billeteras.Handlers;
using Application.UseCases.Billeteras.Queries;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using SubastaYa.Middlewares;

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

// Configuración de ASP.NET Core Identity
builder.Services.AddIdentity<Usuario, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Registramos los Handlers para que puedan ser inyectados en el Mediator
builder.Services.AddScoped<IRequestHandler<ObtenerCategoriasQuery, List<CategoriaDto>>, ObtenerCategoriasQueryHandler>();
builder.Services.AddScoped<IRequestHandler<RegistrarUsuarioCommand, UsuarioDTO>, RegistrarUsuarioCommandHandler>();

builder.Services.AddScoped<IRequestHandler<ObtenerSaldosQuery, BilleteraSaldosDto?>, ObtenerSaldosQueryHandler>();
builder.Services.AddScoped<IRequestHandler<DepositarFondosCommand, BilleteraSaldosDto>, DepositarFondosCommandHandler>();
// Registramos el Mediador: cuando alguien pida IMediator, .NET le entrega una instancia de Mediator
builder.Services.AddScoped<IMediator, Mediator>();

// Para activar los Controllers
builder.Services.AddControllers();

var app = builder.Build();


app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Habilitar la política de CORS (debe ir antes de los endpoints)
app.UseCors("AllowFrontend");

// Habilitar Autenticación y Autorización para Identity
app.UseAuthentication();
app.UseAuthorization();

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
