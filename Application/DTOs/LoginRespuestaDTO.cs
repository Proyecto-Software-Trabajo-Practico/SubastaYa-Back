using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    
    public record LoginRespuestaDTO(
        int Id,
        string Nombre,
        string Email,
        string Token,
        string Mensaje
        );
}
