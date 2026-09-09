using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;


public record PujaDTO(
    int Id,
    decimal Monto,
    DateTime FechaPuja,
    int CompradorId,
    string CompradorNombre
);
