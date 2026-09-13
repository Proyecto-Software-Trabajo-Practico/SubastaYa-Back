using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.UseCases.Pujas.Commands;

public record CrearPujaCommand(
    int SubastaId,
    int CompradorId,
    decimal Monto
);
