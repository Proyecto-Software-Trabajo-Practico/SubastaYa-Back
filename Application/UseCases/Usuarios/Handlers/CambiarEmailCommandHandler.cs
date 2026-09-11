using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuarios.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Usuarios.Handlers;

public class CambiarEmailCommandHandler : IRequestHandler<CambiarEmailCommand, UsuarioDTO>
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CambiarEmailCommandHandler(
        UserManager<Usuario> userManager,
        IAuditoriaRepository auditoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _auditoriaRepository = auditoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UsuarioDTO> HandleAsync(CambiarEmailCommand request, CancellationToken cancellationToken = default)
    {
        var usuario = await _userManager.FindByIdAsync(request.UsuarioId.ToString());
        if (usuario == null)
        {
            throw new KeyNotFoundException($"El usuario con ID {request.UsuarioId} no existe.");
        }

        // 1. Validar si el nuevo email ya está registrado por otro usuario
        var usuarioExistente = await _userManager.FindByEmailAsync(request.NuevoEmail);
        if (usuarioExistente != null && usuarioExistente.Id != usuario.Id)
        {
            throw new InvalidOperationException("El correo electrónico ya está registrado por otra cuenta.");
        }

        // 2. Actualizar Email y UserName de forma coherente
        usuario.Email = request.NuevoEmail;
        usuario.UserName = request.NuevoEmail;

        // 3. Crear el log de auditoría
        var detalle = JsonSerializer.Serialize(new { NuevoEmail = request.NuevoEmail });
        var logAuditoria = new Auditoria(
            entidad: "USUARIO",
            entidadId: usuario.Id,
            accion: "CAMBIO_EMAIL",
            usuarioId: usuario.Id,
            detalleJson: detalle
        );

        await _auditoriaRepository.AddAsync(logAuditoria);

        // 4. UpdateAsync de UserManager guarda las modificaciones del usuario
        var result = await _userManager.UpdateAsync(usuario);
        if (!result.Succeeded)
        {
            var errores = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"No se pudo actualizar el email: {errores}");
        }

        // 5. Asegurar la persistencia del registro de auditoría
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UsuarioDTO(usuario.Id, usuario.Nombre, usuario.Email, usuario.FechaRegistro);
    }
}
