using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs;

public record BilleteraSaldosDto(
    int Id,
    int UsuarioId,
    decimal SaldoTotal,
    decimal SaldoRetenido,
    decimal SaldoDisponible
);