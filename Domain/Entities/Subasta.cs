using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public class Subasta : BaseEntity
{
    public int VendedorId { get; private set; }
    public int CategoriaId { get; private set; }
    
    // Propiedades de navegación
    public virtual Usuario Vendedor { get; private set; } = null!;
    public virtual Categoria Categoria { get; private set; } = null!;
    public virtual ICollection<Puja> Pujas { get; private set; } = new List<Puja>();

    public string Titulo { get; private set; }
    public string Descripcion { get; private set; }
    public string? UrlImagen { get; private set; }
    public decimal PrecioBase { get; private set; }
    public decimal IncrementoMinimo { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }
    public string Estado { get; private set; }
    
    // Control de concurrencia optimista (Optimistic Locking): previene condiciones de carrera 
    // cuando dos postores intentan superar la puja líder exactamente al mismo tiempo.
    public int Version { get; private set; }

    public Subasta(
        int vendedorId, 
        int categoriaId, 
        string titulo, 
        string descripcion, 
        decimal precioBase, 
        decimal incrementoMinimo, 
        DateTime fechaInicio, 
        DateTime fechaFin)
    {
        if (fechaInicio >= fechaFin)
            throw new ArgumentException("La fecha de inicio debe ser anterior a la fecha de fin.");

        VendedorId = vendedorId;
        CategoriaId = categoriaId;
        Titulo = titulo;
        Descripcion = descripcion;
        PrecioBase = precioBase;
        IncrementoMinimo = incrementoMinimo;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        Estado = "PROGRAMADA";
    }

    private Subasta() { 
        Titulo = null!;
        Descripcion = null!;
        Estado = null!;
    } // Para EF Core

    // Regla de Negocio: Activar la subasta
    public void Activar()
    {
        if (Estado != "PROGRAMADA")
            throw new InvalidOperationException("Solo se pueden activar subastas programadas.");
            
        Estado = "ACTIVA";
    }

    public void IncrementarVersion() => Version++;
}
