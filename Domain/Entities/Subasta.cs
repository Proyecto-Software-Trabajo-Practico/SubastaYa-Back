using Domain.Common;
using System;
using System.Collections.Generic;
using Domain.Exceptions;

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
    string? urlImagen,
    decimal precioBase,
    decimal incrementoMinimo,
    DateTime fechaInicio,
    DateTime fechaFin)
    {
        // Invariantes del Dominio: garantizan la integridad financiera de la subasta
        if (precioBase <= 0)
            throw new DomainException("El precio base debe ser mayor a cero.");

        if (incrementoMinimo <= 0)
            throw new DomainException("El incremento mínimo debe ser mayor a cero.");

        // Invariantes Cronológicas: se admite 1 minuto de tolerancia por latencia de red/reloj
        if (fechaInicio < DateTime.UtcNow.AddMinutes(-1))
            throw new DomainException("La fecha de inicio no puede estar en el pasado.");

        if (fechaFin <= fechaInicio)
            throw new DomainException("La fecha de fin debe ser estrictamente posterior a la fecha de inicio.");

        VendedorId = vendedorId;
        CategoriaId = categoriaId;
        Titulo = titulo;
        Descripcion = descripcion;
        UrlImagen = urlImagen;
        PrecioBase = precioBase;
        IncrementoMinimo = incrementoMinimo;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;

        // Regla de Negocio: Si inicia inmediatamente, nace ACTIVA; de lo contrario, PROGRAMADA
        Estado = (fechaInicio <= DateTime.UtcNow) ? "ACTIVA" : "PROGRAMADA";
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
            throw new DomainException("Solo se pueden activar subastas programadas.");
            
        Estado = "ACTIVA";
    }

    public void IncrementarVersion() => Version++;

    /*
     * Regla de Negocio Anti-Sniping (Soft Close / Prórroga Dinámica):
     * Si una puja ingresa dentro de los últimos 60 segundos antes de la finalización
     * de una subasta activa, se prorroga la fecha de cierre sumándole 2 minutos adicionales.
     * 
     * Retorna true si se aplicó la extensión; false en caso contrario.
     * El parámetro 'fechaReferencia' es opcional (por defecto usa DateTime.UtcNow), lo cual
     * permite desacoplar el método del reloj del sistema para escribir pruebas unitarias determinísticas.
     */
    public bool EvaluarExtensionAntiSniping(DateTime? fechaReferencia = null)
    {
        // Regla de salvaguarda: Solo se evalúa sobre subastas actualmente en curso
        if (Estado != "ACTIVA")
            return false;

        var ahora = fechaReferencia ?? DateTime.UtcNow;
        var tiempoRestante = FechaFin - ahora;

        // Si la subasta aún no concluyó y falta 1 minuto (60 segundos) o menos
        if (tiempoRestante > TimeSpan.Zero && tiempoRestante <= TimeSpan.FromSeconds(60))
        {
            FechaFin = FechaFin.AddMinutes(2);
            return true;
        }

        return false;
    }
}
