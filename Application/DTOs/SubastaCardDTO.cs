using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.DTOs;


public record SubastaCardDTO(
    int Id,
    string Titulo,
    string? UrlImagen,
    string Estado,
    decimal PrecioBase,
    decimal PrecioActual,
    int CantidadPujas,
    DateTime FechaFin,
    int CategoriaId,
    string CategoriaNombre
);
