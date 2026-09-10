using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;


public record SubastaDetalleDTO(
    int Id,
    string Titulo,
    string Descripcion,
    string? UrlImagen,
    string Estado,
    decimal PrecioBase,
    decimal IncrementoMinimo,
    decimal PrecioActual,
    int CantidadPujas,
    DateTime FechaInicio,
    DateTime FechaFin,
    int CategoriaId,
    string CategoriaNombre,
    int VendedorId,
    string VendedorNombre,
    List<PujaDTO> UltimasPujas 
);