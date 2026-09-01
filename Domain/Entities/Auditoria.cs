using Domain.Common;
using System;

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

    public Auditoria(string entidad, int entidadId, string accion, string detalleJson, int? usuarioId = null)
    {
        Entidad = entidad;
        EntidadId = entidadId;
        Accion = accion;
        DetalleJson = detalleJson;
        UsuarioId = usuarioId;
    }

    private Auditoria() 
    { 
        Entidad = null!;
        Accion = null!;
        DetalleJson = null!;
    } // Para EF Core
}
