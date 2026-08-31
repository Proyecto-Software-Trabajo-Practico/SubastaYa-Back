using Domain.Common;
using System;

namespace Domain.Entities;

public class Categoria : BaseEntity
{
    public string Nombre { get; private set; }
    public string? UrlIcono { get; private set; }

    public Categoria(string nombre, string? urlIcono = null)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(nombre));

        Nombre = nombre;
        UrlIcono = urlIcono;
    }

    private Categoria() { // Para EF Core 
        Nombre = null!;
    }
}
