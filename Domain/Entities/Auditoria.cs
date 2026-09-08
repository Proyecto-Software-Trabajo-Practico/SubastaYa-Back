using Domain.Common;

namespace Domain.Entities;

public class Auditoria : BaseEntity
{
    public string Entidad { get; private set; }
    public int EntidadId { get; private set; }
    public string Accion { get; private set; }
    public int? UsuarioId { get; private set; }
    public virtual Usuario? Usuario { get; private set; }
    public string DetalleJson { get; private set; }
    public DateTime Fecha { get; private set; }

    public Auditoria(string entidad, int entidadId, string accion, int? usuarioId, string detalleJson)
    {
        Entidad = entidad;
        EntidadId = entidadId;
        Accion = accion;
        UsuarioId = usuarioId;
        DetalleJson = detalleJson;
        Fecha = DateTime.UtcNow;
    }

    private Auditoria()
    {
        Entidad = null!;
        Accion = null!;
        DetalleJson = null!;
    }
}
