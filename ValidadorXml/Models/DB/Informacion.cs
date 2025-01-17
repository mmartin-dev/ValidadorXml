using System;
using System.Collections.Generic;

namespace ValidadorXml.Models.DB;

public partial class Informacion
{
    public int Id { get; set; }

    public string? RfcEmisor { get; set; }

    public string? RfcReceptor { get; set; }

    public string? FolioFiscal { get; set; }

    public DateTime? FechaEmision { get; set; }

    public decimal? Total { get; set; }

    public int? EstatusId { get; set; }

    public virtual Estatus? Estatus { get; set; }
}
