using System.Text.Json;
using Application.Interfaces;
using Application.UseCases.Usuarios.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Usuarios.Handlers;

public class CambiarPasswordCommandHandler : IRequestHandler<CambiarPasswordCommand, bool>
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CambiarPasswordCommandHandler(
        UserManager<Usuario> userManager,
        IAuditoriaRepository auditoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _auditoriaRepository = auditoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(CambiarPasswordCommand request, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(request.UsuarioId.ToString());
        if (usuario == null)
        {
            throw new KeyNotFoundException($"El usuario con ID {request.UsuarioId} no existe.");
        }

        // 1. ChangePasswordAsync valida el hash actual y aplica la nueva contraseña
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

        // 2. Registrar evento de auditoría (sin datos sensibles)
        var detalle = JsonSerializer.Serialize(new { Mensaje = "Cambio de contraseña exitoso" });
        var logAuditoria = new Auditoria(
            entidad: "USUARIO",
            entidadId: usuario.Id,
            accion: "CAMBIO_PASSWORD",
            usuarioId: usuario.Id,
            detalleJson: detalle
        );

        await _auditoriaRepository.AddAsync(logAuditoria);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
