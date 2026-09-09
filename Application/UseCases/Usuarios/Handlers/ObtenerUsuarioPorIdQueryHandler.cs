using Application.DTOs;
using Application.Interfaces;
using Application.UseCases.Usuarios.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Usuarios.Handlers
{
    public class ObtenerUsuarioPorIdQueryHandler : IRequestHandler<ObtenerUsuarioPorIdQuery, UsuarioDTO>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerUsuarioPorIdQueryHandler(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<UsuarioDTO> HandleAsync(ObtenerUsuarioPorIdQuery request, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(request.UsuarioId);

            if (usuario == null)
            {
                throw new KeyNotFoundException($"El usuario con ID {request.UsuarioId} no existe.");
            }

            return new UsuarioDTO(
                usuario.Id,
                usuario.Nombre,
                usuario.Email!,
                usuario.FechaRegistro
            );
        }
    }
}
