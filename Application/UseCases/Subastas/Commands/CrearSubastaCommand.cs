using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Application.DTOs;

namespace Application.UseCases.Subastas.Commands;


public record CrearSubastaCommand(
    int VendedorId,
    int CategoriaId,
    string Titulo,
    string Descripcion,
    string? UrlImagen,
    decimal PrecioBase,
    decimal IncrementoMinimo,
    DateTime FechaInicio,
    DateTime FechaFin
)
{
    // Constructor de conveniencia para mapear directamente desde el DTO recibido en el Controller
    public CrearSubastaCommand(int vendedorId, CrearSubastaDTO dto) : this(
        vendedorId,
        dto.CategoriaId,
        dto.Titulo,
        dto.Descripcion,
        dto.UrlImagen,
        dto.PrecioBase,
        dto.IncrementoMinimo,
        dto.FechaInicio,
        dto.FechaFin
    )
    {
    }
}
