using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using System;

namespace Domain.Entities;

public class Subasta : BaseEntity
{
    public int VendedorId { get; private set; }
    public int CategoriaId { get; private set; }
    
    // Propiedad de navegación
    public virtual Categoria Categoria { get; private set; }

    public string Titulo { get; private set; }
    public string Descripcion { get; private set; }
    public string? UrlImagen { get; private set; }
    public Dinero PrecioBase { get; private set; }
    public Dinero IncrementoMinimo { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }
    public EstadoSubasta Estado { get; private set; }
    
    // Control de concurrencia optimista (Optimistic Locking): previene condiciones de carrera 
    // cuando dos postores intentan superar la puja líder exactamente al mismo tiempo.
    public int Version { get; private set; }

    public Subasta(
        int vendedorId, 
        int categoriaId, 
        string titulo, 
        string descripcion, 
        Dinero precioBase, 
        Dinero incrementoMinimo, 
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
        Estado = EstadoSubasta.Programada;
    }

    private Subasta() { 
        Titulo = null!;
        Descripcion = null!;
        PrecioBase = null!;
        IncrementoMinimo = null!;
        Categoria = null!;
    } // Para EF Core

    // Regla de Negocio: Activar la subasta
    public void Activar()
    {
        if (Estado != EstadoSubasta.Programada)
            throw new InvalidOperationException("Solo se pueden activar subastas programadas.");
            
        Estado = EstadoSubasta.Activa;
    }
}
