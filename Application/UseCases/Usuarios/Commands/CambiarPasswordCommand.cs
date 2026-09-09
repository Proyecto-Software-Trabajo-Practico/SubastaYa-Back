using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Usuarios.Commands
{
    public record CambiarPasswordCommand(
        int UsuarioId,
        string PasswordActual,
        string NuevaPassword);
}
