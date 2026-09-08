using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuario.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RegistrarUsuarioCommandHandler : IRequestHandler<RegistrarUsuarioCommand, UsuarioDTO>
{
    private readonly UserManager<Usuario> _userManager;

    public RegistrarUsuarioCommandHandler(UserManager<Usuario> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UsuarioDTO> HandleAsync(RegistrarUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        // 1. Instanciar la entidad de dominio Usuario
        var usuario = new Usuario(request.Email, request.Nombre);

        // 2. Identity valida automáticamente si el email existe y hashea la contraseña internamente
        var result = await _userManager.CreateAsync(usuario, request.Password);

        // 3. Manejar errores de creación (ej. contraseña débil, email duplicado, etc.)
        if (!result.Succeeded)
        {
            var primerError = result.Errors.FirstOrDefault()?.Description ?? "Error al registrar el usuario.";
            throw new InvalidOperationException(primerError);
        }

        // 4. Retornar el DTO de respuesta
        return new UsuarioDTO(
            usuario.Id,
            usuario.Nombre,
            usuario.Email!,
            usuario.FechaRegistro
        );
    }
}
