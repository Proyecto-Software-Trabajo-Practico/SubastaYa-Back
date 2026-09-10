using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    // DTO (Data Transfer Object) para la respuesta de inicio de sesión,
    // lo usamos para no devolver la entidad completa
    public record LoginRespuestaDTO(
        int Id,
        string Nombre,
        string Email,
        string Token,
        string Mensaje
        );
}
