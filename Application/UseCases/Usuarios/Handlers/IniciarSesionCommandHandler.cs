using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuarios.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Usuarios.Handlers;

public class IniciarSesionCommandHandler : IRequestHandler<IniciarSesionCommand, LoginRespuestaDTO>
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IJwtProvider _jwtProvider;

    public IniciarSesionCommandHandler(
        UserManager<Usuario> userManager,
        IJwtProvider jwtProvider)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
    }

    public async Task<LoginRespuestaDTO> HandleAsync(IniciarSesionCommand request, CancellationToken cancellationToken = default)
    {
        // 1. Buscar el usuario por email
        var usuario = await _userManager.FindByEmailAsync(request.Email);
        if (usuario == null)
        {
            throw new InvalidOperationException("Credenciales inválidas.");
        }

        // 2. Comprobar la contraseña
        var esPasswordValida = await _userManager.CheckPasswordAsync(usuario, request.Password);
        if (!esPasswordValida)
        {
            throw new InvalidOperationException("Credenciales inválidas.");
        }

        // 3. Generar el Token JWT
        var token = _jwtProvider.GenerarToken(usuario);

        // 4. Retornar DTO con el Token
        return new LoginRespuestaDTO(
            usuario.Id,
            usuario.Nombre,
            usuario.Email!,
            token,
            "Inicio de sesión exitoso."
        );
    }
}
