using System;

namespace Application.DTOs;


public record SubastaVendedorDTO(
    int Id,
    string Titulo,
    string? UrlImagen,
    string Estado,
    decimal PrecioBase,
    decimal PrecioActualOFinal,
    decimal TotalRecaudado,
    int CantidadPujas,
    DateTime FechaInicio,
    DateTime FechaFin,
    string EstadoAdjudicacion,
    string CategoriaNombre,
    string? GanadorNombre
);
