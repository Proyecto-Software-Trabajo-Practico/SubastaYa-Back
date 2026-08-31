using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using System;

namespace Domain.Entities;

public class TransaccionLedger : BaseEntity
{
    public int BilleteraId { get; private set; }
    
    // Propiedad de navegación
    public virtual Billetera Billetera { get; private set; }
    
    public TipoTransaccion Tipo { get; private set; }
    public Dinero Monto { get; private set; }
    public DateTime Fecha { get; private set; }
    
    // Opcional, para trazabilidad si la transacción es sobre una subasta
    public int? SubastaId { get; private set; }

    public TransaccionLedger(int billeteraId, TipoTransaccion tipo, Dinero monto, int? subastaId = null)
    {
        BilleteraId = billeteraId;
        Tipo = tipo;
        Monto = monto;
        Fecha = DateTime.UtcNow;
        SubastaId = subastaId;
    }

    private TransaccionLedger() { 
        Monto = null!;
        Billetera = null!;
    } // Para EF Core
}
