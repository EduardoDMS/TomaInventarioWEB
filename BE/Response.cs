using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}
