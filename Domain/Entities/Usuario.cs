using Domain.Common;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Principal;

namespace Domain.Entities;

public class Usuario : IdentityUser<int>
{
    public string Nombre { get; private set; }
    public DateTime FechaRegistro { get; private set; }

    public Usuario(string email, string nombre)
    {
        Email = email;
        UserName = email;
        Nombre = nombre;
        FechaRegistro = DateTime.UtcNow;
    }

    private Usuario() 
    { 
        Nombre = null!;
    }
}
