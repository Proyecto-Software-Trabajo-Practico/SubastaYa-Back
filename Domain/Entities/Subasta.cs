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

    // Control de concurrencia optimista (Optimistic Locking): RowVersion nativo de SQL Server
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

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
        if (precioBase <= 0)
            throw new DomainException("El precio base debe ser mayor a cero.");

        if (incrementoMinimo <= 0)
            throw new DomainException("El incremento mínimo debe ser mayor a cero.");

        if (fechaInicio < DateTime.UtcNow.AddMinutes(-1))
            throw new DomainException("La fecha de inicio no puede estar en el pasado.");

        if (fechaFin <= fechaInicio)
            throw new DomainException("La fecha de fin debe ser strictly posterior a la fecha de inicio.");

        VendedorId = vendedorId;
        CategoriaId = categoriaId;
        Titulo = titulo;
        Descripcion = descripcion;
        UrlImagen = urlImagen;
        PrecioBase = precioBase;
        IncrementoMinimo = incrementoMinimo;
        FechaInicio = fechaInicio;
        FechaFin = fechaFin;

        Estado = (fechaInicio <= DateTime.UtcNow) ? "ACTIVA" : "PROGRAMADA";
    }

    private Subasta()
    {
        Titulo = null!;
        Descripcion = null!;
        Estado = null!;
    }

    public void Activar()
    {
        if (Estado != "PROGRAMADA")
            throw new DomainException("Solo se pueden activar subastas programadas.");

        Estado = "ACTIVA";
    }

    public void Finalizar()
    {
        if (Estado != "ACTIVA")
            throw new DomainException("Solo se pueden finalizar subastas que se encuentren en estado ACTIVA.");

        Estado = "FINALIZADA";
    }

    public void MarcarDesierta()
    {
        if (Estado != "ACTIVA")
            throw new DomainException("Solo se pueden declarar desiertas subastas que se encuentren en estado ACTIVA.");

        Estado = "DESIERTA";
    }

    public bool EvaluarExtensionAntiSniping(DateTime? fechaReferencia = null)
    {
        if (Estado != "ACTIVA")
            return false;

        var ahora = fechaReferencia ?? DateTime.UtcNow;
        var tiempoRestante = FechaFin - ahora;

        if (tiempoRestante > TimeSpan.Zero && tiempoRestante <= TimeSpan.FromSeconds(60))
        {
            FechaFin = FechaFin.AddMinutes(2);
            return true;
        }

        return false;
    }
}