using System;
using System.Collections.Generic;

namespace ValidadorXml.Models.DB;

        public partial class LogError
        {
            public int Id { get; set; }

            public string? Mensaje { get; set; }

            public DateTime? Fecha { get; set; }

            public string? Archivo { get; set; }
        }

