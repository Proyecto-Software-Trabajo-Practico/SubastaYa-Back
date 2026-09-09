using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuarios.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Usuarios.Handlers;

public class CambiarEmailCommandHandler : IRequestHandler<CambiarEmailCommand, UsuarioDTO>
{
    private readonly UserManager<Usuario> _userManager;

    public CambiarEmailCommandHandler(UserManager<Usuario> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UsuarioDTO> HandleAsync(CambiarEmailCommand request, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(request.UsuarioId.ToString());
        if (usuario == null)
        {
            throw new KeyNotFoundException($"El usuario con ID {request.UsuarioId} no existe.");
        }

        // Validar si el nuevo email ya está registrado por otro usuario
        var usuarioExistente = await _userManager.FindByEmailAsync(request.NuevoEmail);
        if (usuarioExistente != null && usuarioExistente.Id != usuario.Id)
        {
            throw new InvalidOperationException("El correo electrónico ya está registrado por otra cuenta.");
        }

        // Actualizar Email y UserName de forma coherente
        usuario.Email = request.NuevoEmail;
        usuario.UserName = request.NuevoEmail;

        var result = await _userManager.UpdateAsync(usuario);
        if (!result.Succeeded)
        {
            var errores = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"No se pudo actualizar el email: {errores}");
        }

        return new UsuarioDTO(usuario.Id, usuario.Nombre, usuario.Email, usuario.FechaRegistro);
    }
}
