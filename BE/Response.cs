using System;
using System.Collections.Generic;

namespace BE
{
    public class Response
    {

        public bool HUBO_ERROR { get; set; }
        public string MENSAJE_ERROR { get; set; }
        public int ERR_CODE { get; set; }
        public object Entity { get; set; }

        public int count { get; set; }
        // si todo se rompe desconmentame y reemplazame--CANELITA
        // public List<decimal> footerTable { get; set; }

        //--CANELITA 19/08/26
        public int CodigoResultado { get; set; } // para poder diferenciar el tipo de errores tranqui solo se usaran en 1 o 4 metodos maximos... si 
        public object footerTable { get; set; }

        //emoreteteee
        public Guid? TokenSesion { get; set; }

        //reporte principal
        public List<string> infoInventario { get; set; }

        //lista errores
        public List<ResultadoErroresImportacion> resultadoErroresImportacion { get; set; }
    }

    public class ResultadoImportacionBE
    {
        public List<ImportBE> Lista { get; set; }

        public int Errores { get; set; }
        public int Actualizados { get; set; }
        public int Nuevos { get; set; }
        public int SinCambios { get; set; }
    }
}
