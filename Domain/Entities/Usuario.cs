using Domain.Common;
using System;

namespace Domain.Entities;

public class Usuario : BaseEntity
{
    public string Email { get; private set; }
    public string Nombre { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime FechaRegistro { get; private set; }

    public Usuario(string email, string nombre, string passwordHash)
    {
        Email = email;
        Nombre = nombre;
        PasswordHash = passwordHash;
    }

    private Usuario() 
    { 
        Email = null!;
        Nombre = null!;
        PasswordHash = null!;
    } // Para EF Core
}
