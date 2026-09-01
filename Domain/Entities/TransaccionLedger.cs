using Domain.Common;
using System;

namespace Domain.Entities;

public class TransaccionLedger : BaseEntity
{
    public int BilleteraId { get; private set; }
    
    // Propiedad de navegación
    public virtual Billetera Billetera { get; private set; } = null!;
    
    public string Tipo { get; private set; }
    public decimal Monto { get; private set; }
    public DateTime Fecha { get; private set; }
    
    // Opcional, para trazabilidad si la transacción es sobre una subasta
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
    } // Para EF Core
}
