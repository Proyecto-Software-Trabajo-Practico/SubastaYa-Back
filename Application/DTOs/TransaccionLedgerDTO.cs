using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.DTOs;


public record TransaccionLedgerDTO(
    int Id,
    int BilleteraId,
    string Tipo,
    decimal Monto,
    DateTime Fecha,
    int? SubastaId
);
