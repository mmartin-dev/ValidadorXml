using System;
using System.Collections.Generic;

namespace ValidadorXml.Models.DB;

public partial class Estatus
{
    public int EstatusId { get; set; }

    public string? Descripcion { get; set; }

    public virtual ICollection<Informacion> Informacions { get; set; } = new List<Informacion>();
}
