using Application.DTOs;
using Application.Interfaces;
using Application.Mediators;
using Application.UseCases.Billeteras.Commands;
using Application.UseCases.Billeteras.Handlers;
using Application.UseCases.Billeteras.Queries;
using Application.UseCases.Categorias.Handlers;
using Application.UseCases.Categorias.Queries;
using Application.UseCases.Usuarios.Commands;
using Application.UseCases.Usuarios.Handlers;
using Application.UseCases.Usuarios.Queries;
using Domain.Entities;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SubastaYa.Middlewares;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Configuración de Swagger con soporte para Bearer Token (JWT)
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SubastaYa API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingresá el token JWT en el formato: Bearer {tu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
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

// Configuración del esquema de Autenticación JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// Registro de Handlers y Mediador
builder.Services.AddScoped<IRequestHandler<ObtenerCategoriasQuery, List<CategoriaDto>>, ObtenerCategoriasQueryHandler>();
builder.Services.AddScoped<IRequestHandler<RegistrarUsuarioCommand, UsuarioDTO>, RegistrarUsuarioCommandHandler>();
builder.Services.AddScoped<IRequestHandler<ObtenerSaldosQuery, BilleteraSaldosDto?>, ObtenerSaldosQueryHandler>();
builder.Services.AddScoped<IRequestHandler<DepositarFondosCommand, BilleteraSaldosDto>, DepositarFondosCommandHandler>();
builder.Services.AddScoped<IRequestHandler<IniciarSesionCommand, LoginRespuestaDTO>, IniciarSesionCommandHandler>();
builder.Services.AddScoped<IRequestHandler<ObtenerUsuarioPorIdQuery, UsuarioDTO>, ObtenerUsuarioPorIdQueryHandler>();
builder.Services.AddScoped<IRequestHandler<CambiarEmailCommand, UsuarioDTO>, CambiarEmailCommandHandler>();
builder.Services.AddScoped<IRequestHandler<CambiarPasswordCommand, bool>, CambiarPasswordCommandHandler>();
builder.Services.AddScoped<IMediator, Mediator>();

// Controllers
builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// El orden importa: Primero quién es, luego qué puede hacer
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();