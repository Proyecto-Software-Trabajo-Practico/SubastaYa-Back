using System;

namespace Application.DTOs;

/*
 * DTO especializado para la vista del panel de vendedor ("Mis Publicaciones" - Módulo 5).
 * Desacopla las necesidades del vendedor (métricas de recaudación y estado de adjudicación)
 * del catálogo público general respetando el principio de Responsabilidad Única (SRP).
 */
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
