using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAT.Services.ConsultaCFDIService;
using SW.Services.Status;
using System.Xml;
using ValidadorXml.Models;
using ValidadorXml.Models.DB;


namespace ValidadorXmls.Controllers
{
    public class HomeController : Controller
    {
        private readonly ValidadorXmlContext _context;
        

        public HomeController(ValidadorXmlContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ValidarXMLCargarAsync(IFormFile[] archivos)
        {
            List<CFDI> xmls = new List<CFDI>();

            for (int i = 0; i < archivos.Length; i++)
            {
                try
                {
                    CFDI xmlActual = new CFDI();
                    byte[] archivoActual = ConvertToBytes(archivos[i]);

                    if (archivoActual.Length == 0)
                    {
                        // Registrar en log si el archivo está vacío
                        var logError = new LogError
                        {
                            Mensaje = "El archivo está vacío.",
                            Fecha = DateTime.Now,
                            Archivo = archivos[i]?.FileName
                        };
                        _context.LogError.Add(logError);
                        await _context.SaveChangesAsync();
                        continue;  // Saltar al siguiente archivo
                    }

                    Stream stream = new MemoryStream(archivoActual);
                    XmlTextReader xmlReader = new XmlTextReader(stream);

                    bool tieneInformacion = false; // Flag para verificar si se encontró alguna información útil en el XML


                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && (xmlReader.Name == "cfdi:Emisor"))
                        {
                            if (xmlReader.HasAttributes)
                                xmlActual.rfc_emisor = xmlReader.GetAttribute("Rfc") ?? xmlReader.GetAttribute("rfc");
                        }

                        if (xmlReader.NodeType == XmlNodeType.Element && (xmlReader.Name == "cfdi:Receptor"))
                        {
                            if (xmlReader.HasAttributes)
                                xmlActual.rfc_receptor = xmlReader.GetAttribute("Rfc") ?? xmlReader.GetAttribute("rfc");
                        }

                        if (xmlReader.NodeType == XmlNodeType.Element && (xmlReader.Name == "cfdi:Comprobante"))
                        {
                            if (xmlReader.HasAttributes)
                            {
                                xmlActual.total = xmlReader.GetAttribute("Total") ?? xmlReader.GetAttribute("total");
                                xmlActual.serie = xmlReader.GetAttribute("Serie") ?? xmlReader.GetAttribute("serie");
                                xmlActual.folio = xmlReader.GetAttribute("Folio") ?? xmlReader.GetAttribute("folio");
                                xmlActual.sello = xmlReader.GetAttribute("Sello") ?? xmlReader.GetAttribute("sello");

                                if (!string.IsNullOrEmpty(xmlActual.total)) tieneInformacion = true;

                            }
                        }

                        if (xmlReader.NodeType == XmlNodeType.Element && (xmlReader.Name == "tfd:TimbreFiscalDigital"))
                        {
                            if (xmlReader.HasAttributes)
                                xmlActual.uuid = xmlReader.GetAttribute("UUID");
                            if (!string.IsNullOrEmpty(xmlActual.uuid)) tieneInformacion = true;

                        }
                    }

                    if (!tieneInformacion)
                    {
                        var logError = new LogError
                        {
                            Mensaje = "El archivo no contiene información válida.",
                            Fecha = DateTime.Now,
                            Archivo = archivos[i]?.FileName
                        };
                        _context.LogError.Add(logError);
                        await _context.SaveChangesAsync();
                        continue;  // Saltar al siguiente archivo
                    }

                    Status status = new Status("https://consultaqr.facturaelectronica.sat.gob.mx/ConsultaCFDIService.svc");
                Acuse response = status.GetStatusCFDI(xmlActual.rfc_emisor, xmlActual.rfc_receptor, xmlActual.total, xmlActual.uuid, xmlActual.sello);
                xmlActual.codigo_estatus = response.CodigoEstatus;
                xmlActual.estado = response.Estado;
                xmlActual.es_cancelable = response.EsCancelable;
                xmlActual.estatus_cancelacion = response.EstatusCancelacion;

                    var estatus = await _context.Estatuses.FirstOrDefaultAsync(e => e.Descripcion == xmlActual.estado);
                    if (estatus == null)
                    {
                        // Crear un nuevo estatus si no existe
                        estatus = new Estatus
                        {
                            Descripcion = xmlActual.estado
                        };
                        _context.Estatuses.Add(estatus);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Actualizar el estatus si ya existe
                        estatus.Descripcion = xmlActual.estado;
                        _context.Estatuses.Update(estatus);
                        await _context.SaveChangesAsync();
                    }

                    // Guardar la información en la tabla "Informacion"
                    var informacion = new Informacion
                    {
                        RfcEmisor = xmlActual.rfc_emisor,
                        RfcReceptor = xmlActual.rfc_receptor,
                        FolioFiscal = xmlActual.uuid,
                        FechaEmision = DateTime.Now, // Cambia según corresponda
                        Total = decimal.TryParse(xmlActual.total, out var totalValue) ? totalValue : 0,
                        EstatusId = estatus.EstatusId
                    };

                    _context.Informacions.Add(informacion);
                    await _context.SaveChangesAsync();

                    xmls.Add(xmlActual);
                }
                catch (Exception ex)
                {
                    // Registrar errores en la tabla LogErrores
                    var logError = new LogError
                    {
                        Mensaje = ex.Message,
                        Fecha = DateTime.Now,
                        Archivo = archivos[i]?.FileName
                    };
                    _context.LogError.Add(logError);
                    await _context.SaveChangesAsync();
                }
            }

            return Json(xmls);
        }

        public byte[] ConvertToBytes(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
