using System.Text.Json;
using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuarios.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Usuarios.Handlers;

public class RegistrarUsuarioCommandHandler : IRequestHandler<RegistrarUsuarioCommand, UsuarioDTO>
{
    private readonly UserManager<Usuario> _userManager;
    private readonly IBilleteraRepository _billeteraRepository;
    private readonly IAuditoriaRepository _auditoriaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegistrarUsuarioCommandHandler(
        UserManager<Usuario> userManager,
        IBilleteraRepository billeteraRepository,
        IAuditoriaRepository auditoriaRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _billeteraRepository = billeteraRepository;
        _auditoriaRepository = auditoriaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UsuarioDTO> HandleAsync(RegistrarUsuarioCommand request, CancellationToken cancellationToken = default)
    {
        var usuario = new Usuario(request.Email, request.Nombre);

        var result = await _userManager.CreateAsync(usuario, request.Password);
        if (!result.Succeeded)
        {
            var errores = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"No se pudo registrar el usuario: {errores}");
        }

        var billetera = new Billetera(usuario.Id);
        await _billeteraRepository.AddAsync(billetera);

        var detalleAuditoria = JsonSerializer.Serialize(new
        {
            Email = usuario.Email,
            Nombre = usuario.Nombre,
            FechaRegistro = usuario.FechaRegistro
        });

        var logAuditoria = new Auditoria(
            entidad: "USUARIO",
            entidadId: usuario.Id,
            accion: "REGISTRO_USUARIO",
            usuarioId: usuario.Id,
            detalleJson: detalleAuditoria
        );
        await _auditoriaRepository.AddAsync(logAuditoria);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new UsuarioDTO(usuario.Id, usuario.Nombre, usuario.Email!, usuario.FechaRegistro);
    }
}
