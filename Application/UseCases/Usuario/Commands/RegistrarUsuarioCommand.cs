using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario.Commands
{
    public record RegistrarUsuarioCommand(
        string Nombre, 
        string Email, 
        string Password
        );
}
