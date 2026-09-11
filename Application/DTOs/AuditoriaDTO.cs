using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public record AuditoriaDTO(
    int Id,
    string Entidad,
    int EntidadId,
    string Accion,
    int? UsuarioId,
    string DetalleJson,
    DateTime Fecha);
}
