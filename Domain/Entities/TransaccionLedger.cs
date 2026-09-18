using Domain.Common;
using System;

namespace Domain.Entities;

public class TransaccionLedger : BaseEntity
{
    public int BilleteraId { get; private set; }
    
    public virtual Billetera Billetera { get; private set; } = null!;
    
    public string Tipo { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime Fecha { get; private set; }
    
    public int? SubastaId { get; private set; }

    public TransaccionLedger(int billeteraId, string tipo, decimal monto, int? subastaId = null)
    {
        BilleteraId = billeteraId;
        Tipo = tipo;
        Monto = monto;
        Fecha = DateTime.UtcNow;
        SubastaId = subastaId;
    }

    private TransaccionLedger() { 
        Tipo = null!;
    } 
}
