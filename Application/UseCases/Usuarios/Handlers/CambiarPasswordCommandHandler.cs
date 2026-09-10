using Application.Interfaces;
using Application.UseCases.Usuarios.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Usuarios.Handlers;

public class CambiarPasswordCommandHandler : IRequestHandler<CambiarPasswordCommand, bool>
{
    private readonly UserManager<Usuario> _userManager;

    public CambiarPasswordCommandHandler(UserManager<Usuario> userManager)
    {
        _userManager = userManager;
    }

    public async Task<bool> HandleAsync(CambiarPasswordCommand request, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(request.UsuarioId.ToString());
        if (usuario == null)
        {
            throw new KeyNotFoundException($"El usuario con ID {request.UsuarioId} no existe.");
        }

        // ChangePasswordAsync valida el hash actual y aplica la nueva contraseña
        var result = await _userManager.ChangePasswordAsync(
            usuario,
            request.PasswordActual,
            request.NuevaPassword
        );

        if (!result.Succeeded)
        {
            var errores = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"No se pudo cambiar la contraseña: {errores}");
        }

        return true;
    }
}
