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
        public List<decimal> footerTable { get; set; }

        //reporte principal
        public List<string> infoInventario { get; set; }
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
