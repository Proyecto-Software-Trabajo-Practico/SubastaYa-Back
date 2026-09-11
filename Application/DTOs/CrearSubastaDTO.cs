using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;


/*Desacoplamiento:
No incluye VendedorId porque vamos a extraerlo con seguridad desde el token JWT en el controlador.
No incluye Estado, Id ni Version, ya que son datos del sistema protegidos por el dominio.
Define UrlImagen como string? para brindar flexibilidad al frontend.*/


public record CrearSubastaDTO(
    string Titulo,
    string Descripcion,
    string? UrlImagen,
    decimal PrecioBase,
    decimal IncrementoMinimo,
    DateTime FechaInicio,
    DateTime FechaFin,
    int CategoriaId
);
